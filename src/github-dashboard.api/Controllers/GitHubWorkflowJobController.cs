using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Azure.Storage.Blobs;
using githubdashboard.api.Models.WorkflowJob;

namespace githubdashboard.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GitHubWorkflowJobController : ControllerBase
{
    private readonly ILogger<GitHubWorkflowJobController> _logger;
    private readonly IConfiguration _configuration;

    public GitHubWorkflowJobController(ILogger<GitHubWorkflowJobController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet]
    public string Get()
    {
        return "GitHub Workflow Job";
    }

    [HttpPost]
    public IActionResult Post(dynamic data)
    {
        _logger.LogInformation($"Run API GitHubWorkflowJobController");

        string connectionString = _configuration["ConnectionStrings:BlobStorage"];

        // Convert json to object
        GitHubWorkflowJob obj = JsonConvert.DeserializeObject<GitHubWorkflowJob>(data.ToString());

        if(obj != null)
        {
            if(obj.workflow_job != null)
            {
                if(obj.workflow_job.status == "completed")
                {
                    // Create a BlobServiceClient object which will be used to create a container client
                    BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                    // Create the container and return a container client object
                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("github-workflow-job");
                    containerClient.CreateIfNotExists();

                    string blobName = string.Empty;
                    if((!String.IsNullOrEmpty(obj.action)) && (obj.workflow_job != null))
                        blobName =  Guid.NewGuid().ToString() + "_" + obj.workflow_job.run_id + "_" + obj.action.ToString() + "_JOB.json";
                    else
                        blobName = Guid.NewGuid().ToString() + "_JOB.json";

                    BlobClient blob = containerClient.GetBlobClient(blobName);

                    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(data.ToString())))
                    {
                        _logger.LogInformation($"Save json file to blob storage - {blobName}");
                        blob.UploadAsync(ms);
                    }
                }
                else
                {
                    _logger.LogInformation($"workflow_job status different completed -> {obj.workflow_job.status}");
                }
            }
            else
            {
                _logger.LogInformation("obj.workflow_job is null");
            }
        }
        else
        {
            _logger.LogInformation("Obj is null");
        }

        return new OkResult();
    }
}
