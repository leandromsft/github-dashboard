using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;

namespace githubdashboard.function.Models.EF
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public DbSet<Issue> Issue { get; set; }
        public DbSet<WorkflowRun> WorkflowRun { get; set; }
        public DbSet<WorkflowJob> WorkflowJob { get; set; }
        public DbSet<WorkflowJobStep> WorkflowJobStep { get; set; }
        public DbSet<ReferencedWorkflows> ReferencedWorkflows { get; set; }
    }
}