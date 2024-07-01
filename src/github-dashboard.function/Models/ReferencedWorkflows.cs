using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace githubdashboard.function.Models.EF
{
    public class ReferencedWorkflows
    {
        [Key]
        public int Id { get; set; }
        public long WorkflowRunId { get; set; }
        public string ReferencedWorkflowPath { get; set; }
        public string Sha { get; set; }
        public string Ref { get; set; }
    }
}