using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Azure.Storage.Blobs;
using githubdashboard.api.Models.WorkflowRun;

namespace githubdashboard.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GitHubWorkflowRunController : ControllerBase
{
    private readonly ILogger<GitHubWorkflowRunController> _logger;
    private readonly IConfiguration _configuration;

    public GitHubWorkflowRunController(ILogger<GitHubWorkflowRunController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet]
    public string Get()
    {
        return "GitHub Workflow Run";
    }

    [HttpPost]
    public IActionResult Post(dynamic data)
    {
        _logger.LogInformation($"Run API GitHubWorkflowRunController");

        string connectionString = _configuration["ConnectionStrings:BlobStorage"];

        // Convert json to object
        GitHubWorkflowRun obj = JsonConvert.DeserializeObject<GitHubWorkflowRun>(data.ToString());

        if(obj != null)
        {
            if(obj.workflow_run != null)
            {
                if(obj.workflow_run.status == "completed")
                {
                    // Create a BlobServiceClient object which will be used to create a container client
                    BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                    // Create the container and return a container client object
                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("github-workflow-run");
                    containerClient.CreateIfNotExists();

                    string blobName = string.Empty;
                    if((!String.IsNullOrEmpty(obj.action)) && (obj.workflow_run != null))
                        blobName =  Guid.NewGuid().ToString() + "_" + obj.workflow_run.id + "_" + obj.action.ToString() + "_WORKFLOW.json";
                    else
                        blobName = Guid.NewGuid().ToString() + "_WORKFLOW.json";

                    BlobClient blob = containerClient.GetBlobClient(blobName);

                    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(data.ToString())))
                    {
                        _logger.LogInformation($"Save json file to blob storage - {blobName}");
                        blob.UploadAsync(ms);
                    }            
                }
                else
                {
                    _logger.LogInformation($"workflow_run status different completed -> {obj.workflow_run.status}");
                }
            }
            else
            {
                _logger.LogInformation("obj.workflow_run is null");
            }
        }
        else
        {
            _logger.LogInformation("Obj is null");
        }

        return new OkResult();
    }
}
