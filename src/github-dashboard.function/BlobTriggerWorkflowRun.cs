using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using githubdashboard.function.ViewModels.WorkflowRun;
using githubdashboard.function.Models.EF;

namespace githubdashboard.function
{
    public class BlobTriggerWorkflowRun
    {
        private readonly MyDbContext dbContext;

        public BlobTriggerWorkflowRun(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [FunctionName("BlobTriggerWorkflowRun")]
        public void Run([BlobTrigger("github-workflow-run/{name}", Connection = "strgithubapp_STORAGE")]Stream myBlob, string name, ILogger log)
        {
            try
            {
                log.LogInformation($"Blob Name: {name} - Size: {myBlob.Length} Bytes");
                StreamReader reader = new StreamReader(myBlob);
                string content = reader.ReadToEnd();
                //log.LogInformation($"content:{content}");

                if(!String.IsNullOrEmpty(content))
                {
                    // Convert json to object
                    GitHubWorkflowRun obj = JsonSerializer.Deserialize<GitHubWorkflowRun>(content.ToString());
                    if(obj != null)
                    {
                        log.LogInformation($"Action: {obj.action} - Workflow Name: {obj.workflow.name}");

                        if(obj.workflow_run != null)
                        {
                            if(obj.workflow_run.status == "completed")
                            {
                                log.LogInformation($"Workflow RunId: {obj.workflow_run.id} - Status: {obj.workflow_run.status}");
                                log.LogInformation("Save workflow_run to database");
                                
                                var run = new githubdashboard.function.Models.EF.WorkflowRun 
                                {
                                    WorkflowRunId = obj.workflow_run.id,
                                    WorkflowRunName = obj.workflow_run.name,
                                    WorkflowRunEvent = obj.workflow_run.@event,
                                    WorkflowRunStatus = obj.workflow_run.status,
                                    WorkflowRunConclusion = obj.workflow_run.conclusion,
                                    WorkflowRunCreateAt = obj.workflow_run.created_at,
                                    WorkflowRunUpdateAt = obj.workflow_run.updated_at,
                                    WorkflowRunAttemp = obj.workflow_run.run_attempt,
                                    WorkflowRunActorLogin = obj.workflow_run.actor.login,
                                    RepositoryName = obj.workflow_run.repository.name
                                };

                                dbContext.WorkflowRun.Add(run);
                                dbContext.SaveChanges();

                                if(obj.workflow_run.referenced_workflows != null)
                                {
                                    log.LogInformation("Save referenced_workflows to database");
                                    
                                    foreach(var referenced in obj.workflow_run.referenced_workflows)
                                    {
                                        var referencedWorkflows = new githubdashboard.function.Models.EF.ReferencedWorkflows
                                        {
                                            WorkflowRunId = obj.workflow_run.id,
                                            ReferencedWorkflowPath = referenced.path,
                                            Sha = referenced.sha,
                                            Ref = referenced.@ref
                                        };

                                        dbContext.ReferencedWorkflows.Add(referencedWorkflows);
                                        dbContext.SaveChanges();
                                    }
                                }
                                else
                                {
                                    log.LogInformation("referenced_workflows is null");
                                }
                            }
                            else
                            {
                                log.LogInformation($"workflow_run status different completed -> {obj.workflow_run.status}");
                            }
                        }
                        else
                        {
                            log.LogInformation("workflow_run is null");
                        }
                    }
                    else
                    {
                        log.LogInformation("Obj is null");
                    }
                }
                else
                {
                    log.LogInformation("content is null");
                }
            }
            catch(Exception ex)
            {
                log.LogError($"ERROR: {ex.ToString()}");
            }
        }
    }
}
