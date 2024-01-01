using Elsa.Http;
using Elsa.Workflows.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ElsaWeb.Controllers;

[ApiController]
[Route("run-workflow")]
public class RunWorkflowController(IWorkflowRunner workflowRunner) : ControllerBase
{
    [HttpGet]
    public async Task Get()
    {
        await workflowRunner.RunAsync(new WriteHttpResponse
        {
            Content = new("Hello ASP.NET world!")
        });
    }
}