using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace githubdashboard.function.Models.EF
{
    public class Issue
    {
        [Key]
        public int Id { get; set; }
        public int IssueId { get; set; }
        public int IssueNumber { get; set; }
        public string? IssueTitle { get; set; }
        public string? Action { get; set; }
        public string? State { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string? RepositoryName { get; set; }
    }
}