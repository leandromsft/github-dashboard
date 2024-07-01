using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using githubdashboard.function.ViewModels.WorkflowJob;
using githubdashboard.function.Models.EF;
using System.Linq;
using System.Linq.Expressions;

namespace githubdashboard.function
{
    public class BlobTriggerWorkflowJob
    {
        private readonly MyDbContext dbContext;

        public BlobTriggerWorkflowJob(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [FunctionName("BlobTriggerWorkflowJob")]
        public void Run([BlobTrigger("github-workflow-job/{name}", Connection = "strgithubapp_STORAGE")]Stream myBlob, string name, ILogger log)
        {
            //https://stackoverflow.com/questions/42129429/how-to-delete-a-blob-using-azure-functions
            try
            {
                log.LogInformation($"Blob Name:{name} - Size: {myBlob.Length} Bytes");
                StreamReader reader = new StreamReader(myBlob);
                string content = reader.ReadToEnd();
                //log.LogInformation($"content:{content}");

                if(!String.IsNullOrEmpty(content))
                {
                    // Convert json to object
                    GitHubWorkflowJob obj = JsonSerializer.Deserialize<GitHubWorkflowJob>(content.ToString());

                    if(obj != null)
                    {
                        if(obj.workflow_job != null)
                        {
                            if(obj.workflow_job.status == "completed")
                            {
                                log.LogInformation($"Action: {obj.action} - Job Name: {obj.workflow_job.name}");
                                log.LogInformation("Create a new WorkflowJob object");

                                var job = new githubdashboard.function.Models.EF.WorkflowJob
                                {
                                    WorkflowJobId = obj.workflow_job.id,      //workflow_job.id
                                    WorkflowRunId = obj.workflow_job.run_id,  // workflow_job.run_id
                                    WorkflowJobName = obj.workflow_job.name,
                                    WorkflowJobAttempt = obj.workflow_job.run_attempt,
                                    WorkflowJobStatus = obj.workflow_job.status,
                                    WorkflowJobConclusion = obj.workflow_job.conclusion,
                                    WorkflowJobStartedAt = obj.workflow_job.started_at,
                                    WorkflowJobCompletedAt = obj.workflow_job.completed_at,
                                    WorkflowJobRunnerId = obj.workflow_job.runner_id,
                                    WorkflowJobRunnerName = obj.workflow_job.runner_name,
                                    WorkflowJobRunnerGroupId = obj.workflow_job.runner_group_id,
                                    WorkflowJobRunnerGroupName = obj.workflow_job.runner_group_name
                                };

                                if(obj.workflow_job.labels != null)
                                {
                                    log.LogInformation("Labels is different null");
                                    if(obj.workflow_job.labels.Count > 0)
                                    {
                                        log.LogInformation($"Add the label {obj.workflow_job.labels[0].ToString()} to workflow_job object");
                                        job.WorkflowJobRunnerLabel = obj.workflow_job.labels[0].ToString();
                                    }
                                    else
                                    {
                                        log.LogInformation($"Labels is empty - {obj.workflow_job.labels.Count}");
                                    }
                                }

                                log.LogInformation("Verify has deployment");
                                if(obj.deployment != null)
                                {    
                                    log.LogInformation($"Add deployment {obj.deployment.environment} to workflow_job object");
                                    job.WorkflowJobIsEnvironment = true;
                                    job.WorkflowJobEnvironmentName = obj.deployment.environment;
                                }
                                else
                                {
                                    log.LogInformation("Deployment is null");
                                }
                                
                                log.LogInformation("Save workflow_job to database");
                                dbContext.WorkflowJob.Add(job);
                                dbContext.SaveChanges();

                                log.LogInformation("Verify has workflow_job.steps");
                                if(obj.workflow_job.steps != null)
                                {
                                    log.LogInformation($"Steps Count: {obj.workflow_job.steps.Count }");

                                    foreach(githubdashboard.function.ViewModels.WorkflowJob.Step st in obj.workflow_job.steps)
                                    {
                                        log.LogInformation("create a new WorkflowJobStep object");

                                        githubdashboard.function.Models.EF.WorkflowJobStep jobStep = new githubdashboard.function.Models.EF.WorkflowJobStep
                                        {
                                            WorkflowJobId = obj.workflow_job.id,
                                            StepName = st.name,
                                            StepStatus = st.status,
                                            StepConclusion = st.conclusion,
                                            StepNumber = st.number,
                                            StepStartedAt = st.started_at,
                                            StepCompletedAt = st.completed_at
                                        };

                                        log.LogInformation($"Save WorkflowJobStep {st.name} to database");
                                        dbContext.WorkflowJobStep.Add(jobStep);
                                        dbContext.SaveChanges();
                                    }
                                }
                                else
                                {
                                    log.LogInformation("workflow_job.steps is null");
                                }
                            }
                            else
                            {
                                log.LogInformation($"workflow_job status different completed - {obj.workflow_job.status}");
                            }
                        }
                        else
                        {
                            log.LogInformation("workflow_job is null");
                        }
                    }
                    else
                    {
                        log.LogInformation("obj is null");
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
