using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace githubdashboard.function.Models.EF
{
    public class WorkflowJobStep
    {
        [Key]
        public int Id { get; set; }
        public long WorkflowJobId { get; set; } //workflow_job.id
        public string? StepName { get; set; }
        public string? StepStatus { get; set; }
        public string? StepConclusion { get; set; }
        public int StepNumber { get; set; }
        public DateTime? StepStartedAt { get; set; }
        public DateTime? StepCompletedAt { get; set; }
    }
}