using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace githubdashboard.function.Models.EF
{
    public class WorkflowJob
    {
        [Key]
        public int Id { get; set; }
        public long WorkflowJobId { get; set; } //workflow_job.id
        public long WorkflowRunId { get; set; } //workflow_job.run_id
        public string? WorkflowJobName { get; set; }
        public int WorkflowJobAttempt { get; set; }
        public string? WorkflowJobStatus { get; set; }
        public string? WorkflowJobConclusion { get; set; }
        public DateTime? WorkflowJobStartedAt { get; set; }
        public DateTime? WorkflowJobCompletedAt { get; set; }
        public int? WorkflowJobRunnerId { get; set; }
        public string? WorkflowJobRunnerName { get; set; }
        public int? WorkflowJobRunnerGroupId { get; set; }
        public string? WorkflowJobRunnerGroupName { get; set; }
        
        //public List<WorkflowJobStep> WorkflowJobStep { get; set; }

        public string? WorkflowJobRunnerLabel { get; set; }

        public bool WorkflowJobIsEnvironment { get; set; } = false;

        public string? WorkflowJobEnvironmentName { get; set; }
    }
}