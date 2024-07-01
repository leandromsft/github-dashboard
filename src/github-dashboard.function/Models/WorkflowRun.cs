using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace githubdashboard.function.Models.EF
{
    public class WorkflowRun
    {
        [Key]
        public int Id { get; set; }
        public long WorkflowRunId { get; set; }
        public string? WorkflowRunName { get; set; }
        public string? WorkflowRunEvent { get; set; }
        public string? WorkflowRunStatus { get; set; }
        public string? WorkflowRunConclusion { get; set; }
        public DateTime WorkflowRunCreateAt { get; set; }
        public DateTime WorkflowRunUpdateAt { get; set; }
        public int WorkflowRunAttemp { get; set; }
        public string? WorkflowRunActorLogin { get; set; }
        public string? RepositoryName { get; set; }
    }
}