using Onboarding.Data;
using Onboarding.Entities;
using Onboarding.Models;
using Microsoft.AspNetCore.Mvc;

namespace Onboarding.Controllers;

[ApiController]
[Route("api/webhooks")]
public class WebhookController(OnboardingDbContext dbContext) : Controller
{
    [HttpPost("run-task")]
    public async Task<IActionResult> RunTask(WebhookEvent webhookEvent)
    {
        var payload = webhookEvent.Payload;
        var taskPayload = payload.TaskPayload;
        var employee = taskPayload.Employee;

        var task = new OnboardingTask
        {
            ProcessId = payload.WorkflowInstanceId,
            ExternalId = payload.TaskId,
            Name = payload.TaskName,
            Description = taskPayload.Description,
            EmployeeEmail = employee.Email,
            EmployeeName = employee.Name,
            CreatedAt = DateTimeOffset.Now
        };

        await dbContext.Tasks.AddAsync(task);
        await dbContext.SaveChangesAsync();

        return Ok();
    }
}