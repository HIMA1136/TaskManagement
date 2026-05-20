using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Infrastructure.Persistence;
using TaskManagement.Infrastructure.Persistence.Entities;

namespace TaskManagement.Infrastructure.Services;

public sealed class AuditService(
    ApplicationDbContext context,
    ICurrentUserService currentUserService,
    ICorrelationIdService correlationIdService) : IAuditService
{
    public async Task LogAsync(
        string entityName,
        string entityId,
        string action,
        string? oldValues,
        string? newValues,
        CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog
        {
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            OldValues = oldValues,
            NewValues = newValues,
            UserId = currentUserService.UserId,
            CorrelationId = correlationIdService.GetOrCreate(),
            Timestamp = DateTime.UtcNow
        };

        await context.AuditLogs.AddAsync(auditLog, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
