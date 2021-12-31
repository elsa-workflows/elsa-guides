using System;
using System.Threading.Tasks;
using Elsa.Services;
using Elsa.Services.Models;
using ElsaGuides.PollingWorkflow.Workflow;
using Microsoft.AspNetCore.Mvc;

namespace ElsaGuides.PollingWorkflow.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IBuildsAndStartsWorkflow _workflow;
        private readonly IWorkflowLaunchpad _workflowLaunchpad;

        public TestController(IBuildsAndStartsWorkflow workflow, IWorkflowLaunchpad workflowLaunchpad)
        {
            _workflow = workflow;
            _workflowLaunchpad = workflowLaunchpad;
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start()
        {
            var correlationId = Guid.NewGuid().ToString();

            await _workflow.BuildAndStartWorkflowAsync<Workflow.PollingWorkflow>(correlationId: correlationId);
            
            return Ok(correlationId);
        }
        
        [HttpPost("abort/{id}")]
        public async Task<IActionResult> Abort(string id)
        {
            var query = new WorkflowsQuery(nameof(AbortActivity), new AbortActivityBookmark(), id);
            
            await _workflowLaunchpad.CollectAndDispatchWorkflowsAsync(query);

            return NoContent();
        }
    }
}