namespace TaskManagement.Infrastructure.Persistence.Entities;

public sealed class IdempotencyKey
{
    public Guid Key { get; init; }
    public string RequestName { get; init; } = string.Empty;
    public string Response { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
