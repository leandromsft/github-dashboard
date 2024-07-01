using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using githubdashboard.function.ViewModels.Issue;
using githubdashboard.function.Models.EF;

namespace githubdashboard.function
{
    public class BlobTriggerIssue
    {
        private readonly MyDbContext dbContext;

        public BlobTriggerIssue(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [FunctionName("BlobTriggerIssue")]
        public void Run([BlobTrigger("github-issue/{name}", Connection = "strgithubapp_STORAGE")]Stream myBlob, string name, ILogger log)
        {
            try
            {

                log.LogInformation($"Blob Name:{name} - Size: {myBlob.Length} Bytes");
                StreamReader reader = new StreamReader(myBlob);
                string content = reader.ReadToEnd();
                //log.LogInformation($"content:{content}");

                if(!String.IsNullOrEmpty(content))
                {
                    // Convert json to object
                    GitHubIssue obj = JsonSerializer.Deserialize<GitHubIssue>(content.ToString());
                    if(obj != null)
                    {
                        if(obj.issue != null)
                        {
                            log.LogInformation($"Action:{obj.action} - Issue Title: {obj.issue.title}");

                            log.LogInformation("Save issue to database");
                            var i = new githubdashboard.function.Models.EF.Issue 
                            { 
                                IssueId=obj.issue.id, 
                                IssueNumber=obj.issue.number, 
                                IssueTitle=obj.issue.title, 
                                Action=obj.action, 
                                State=obj.issue.state, 
                                CreateAt=obj.issue.created_at, 
                                UpdateAt=obj.issue.updated_at,
                                ClosedAt=obj.issue.closed_at,
                                RepositoryName=obj.repository.name
                            };
                            
                            dbContext.Issue.Add(i);
                            dbContext.SaveChanges();
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