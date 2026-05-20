namespace TaskManagement.Application.Common.Interfaces;

public interface IAuditService
{
    Task LogAsync(string entityName, string entityId, string action,
        string? oldValues, string? newValues, CancellationToken cancellationToken = default);
}
