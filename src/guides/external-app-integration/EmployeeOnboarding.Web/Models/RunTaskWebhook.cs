namespace EmployeeOnboarding.Web.Models;

public record RunTaskWebhook(string WorkflowInstanceId, string TaskId, string TaskName, TaskPayload TaskPayload);