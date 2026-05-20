namespace TaskManagement.Infrastructure.Persistence.Entities;

public sealed class AuditLog
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string EntityName { get; init; } = string.Empty;
    public string EntityId { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public string? OldValues { get; init; }
    public string? NewValues { get; init; }
    public string? UserId { get; init; }
    public string? CorrelationId { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
