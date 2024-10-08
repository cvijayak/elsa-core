namespace Elsa.Webhooks.Models;

/// <summary>
/// Stores payload information about the RunTask webhook event type.
/// </summary>
public record RunTaskWebhookPayload(
    string WorkflowInstanceId, 
    string WorkflowDefinitionId, 
    string? WorkflowName, 
    string? CorrelationId,
    string TaskId, 
    string TaskName, 
    object? TaskPayload);