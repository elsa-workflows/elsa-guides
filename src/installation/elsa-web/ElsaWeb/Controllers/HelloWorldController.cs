using Elsa.Http;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Contracts;
using Elsa.Workflows.Memory;
using Microsoft.AspNetCore.Mvc;

namespace ElsaWeb.Controllers;

[ApiController]
[Route("hello-world")]
public class HelloWorldController(IWorkflowRunner workflowRunner) : ControllerBase
{
    [HttpGet]
    public async Task Get([FromQuery] string message = "Hello ASP.NET world!")
    {
        var messageVariable = new Variable<string>(message);
        var workflow = new Sequence
        {
            Variables = { messageVariable },
            Activities =
            {
                new WriteLine("This workflow runs a sequence of steps."),
                new WriteHttpResponse
                {
                    Content = new(message)
                },
                new WriteLine(messageVariable),
                new WriteLine("This workflow is now complete.")
            }
        };

        await workflowRunner.RunAsync(workflow);
    }
}