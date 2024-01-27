namespace Onboarding.Models;

public record WebhookEvent(string EventType, RunTaskWebhook Payload, DateTimeOffset Timestamp);
public record RunTaskWebhook(string WorkflowInstanceId, string TaskId, string TaskName, TaskPayload TaskPayload);
public record TaskPayload(Employee Employee, string Description);
public record Employee(string Name, string Email);