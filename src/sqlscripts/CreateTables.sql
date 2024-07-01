if not exists (select * from sysobjects where name='Issue' and xtype='U')
    CREATE TABLE Issue (
        Id INT PRIMARY KEY IDENTITY (1, 1),
        IssueId INT NOT NULL,
        IssueNumber INT NOT NULL,
        IssueTitle VARCHAR(MAX) NOT NULL,
        Action VARCHAR(255) NOT NULL,
        State VARCHAR(255) NOT NULL,
        CreateAt DATETIME2(7) NOT NULL,
        UpdateAt DATETIME2(7) NULL,
        ClosedAt DATETIME2(7) NULL,
        RepositoryName VARCHAR(MAX) NOT NULL
    )
GO

if not exists (select * from sysobjects where name='WorkflowRun' and xtype='U')
    CREATE TABLE WorkflowRun (
        Id INT PRIMARY KEY IDENTITY (1, 1),
        WorkflowRunId BIGINT NOT NULL,
        WorkflowRunName VARCHAR(255) NOT NULL,
        WorkflowRunEvent VARCHAR(255) NOT NULL,
        WorkflowRunStatus VARCHAR(255) NOT NULL,
        WorkflowRunConclusion VARCHAR(255) NULL,
        WorkflowRunCreateAt DATETIME2(7) NOT NULL,
        WorkflowRunUpdateAt DATETIME2(7) NOT NULL,
        WorkflowRunAttemp INT NOT NULL,
        WorkflowRunActorLogin VARCHAR(255) NOT NULL,
        RepositoryName VARCHAR(255) NOT NULL
    );
GO

if not exists (select * from sysobjects where name='WorkflowJob' and xtype='U')
    CREATE TABLE WorkflowJob (
        Id INT PRIMARY KEY IDENTITY (1, 1),
        WorkflowJobId BIGINT NOT NULL,
        WorkflowRunId BIGINT NOT NULL,
        WorkflowJobName VARCHAR(255) NOT NULL,
        WorkflowJobAttempt INT NOT NULL,
        WorkflowJobStatus VARCHAR(255) NOT NULL,
        WorkflowJobConclusion VARCHAR(255) NULL,
        WorkflowJobStartedAt DATETIME2(7) NOT NULL,
        WorkflowJobCompletedAt DATETIME2(7) NULL,
        WorkflowJobRunnerId INT NOT NULL,
        WorkflowJobRunnerName VARCHAR(255) NOT NULL,
        WorkflowJobRunnerGroupId INT NOT NULL,
        WorkflowJobRunnerGroupName VARCHAR(255) NOT NULL,
        WorkflowJobRunnerLabel VARCHAR(255) NOT NULL,
        WorkflowJobIsEnvironment BIT DEFAULT 0 NOT NULL,
        WorkflowJobEnvironmentName VARCHAR(255) NOT NULL
    )
GO


if not exists (select * from sysobjects where name='WorkflowJobStep' and xtype='U')
    CREATE TABLE WorkflowJobStep (
        Id  INT PRIMARY KEY IDENTITY (1, 1),
        WorkflowJobId BIGINT NOT NULL,
        StepName VARCHAR(255) NOT NULL,
        StepStatus VARCHAR(255) NOT NULL,
        StepConclusion VARCHAR(255) NOT NULL,
        StepNumber INT NOT NULL,
        StepStartedAt DATETIME2(7) NOT NULL,
        StepCompletedAt DATETIME2(7) NULL
    )
GO


if not exists (select * from sysobjects where name='ReferencedWorkflows' and xtype='U')
    CREATE TABLE ReferencedWorkflows (
        Id INT PRIMARY KEY IDENTITY (1, 1),
        WorkflowRunId BIGINT NOT NULL,
        ReferencedWorkflowPath VARCHAR(255) NOT NULL,
        Sha VARCHAR(255) NOT NULL,
        Ref VARCHAR(255) NOT NULL
    )
GO
