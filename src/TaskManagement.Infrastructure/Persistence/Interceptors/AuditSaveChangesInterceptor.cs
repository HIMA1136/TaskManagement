using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Common.Base;
using TaskManagement.Domain.Common.Markers;
using TaskManagement.Infrastructure.Persistence.Entities;

namespace TaskManagement.Infrastructure.Persistence.Interceptors;

public sealed class AuditSaveChangesInterceptor(
    ICurrentUserService currentUserService,
    ICorrelationIdService correlationIdService) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditFields(eventData.Context);
        CreateAuditLogs(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditFields(eventData.Context);
        CreateAuditLogs(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditFields(DbContext? context)
    {
        if (context is null) return;

        var now = DateTime.UtcNow;
        var userId = currentUserService.UserId ?? "system";

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is not IAuditable) continue;

            if (entry.State == EntityState.Added)
            {
                entry.Property("CreatedAt").CurrentValue = now;
                entry.Property("CreatedBy").CurrentValue = userId;
            }

            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                entry.Property("UpdatedAt").CurrentValue = now;
                entry.Property("UpdatedBy").CurrentValue = userId;
            }
        }
    }

    private void CreateAuditLogs(DbContext? context)
    {
        if (context is null) return;

        var correlationId = correlationIdService.GetOrCreate();
        var userId = currentUserService.UserId;

        var auditEntries = context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(e => new AuditLog
            {
                EntityName = e.Entity.GetType().Name,
                EntityId = e.Properties
                    .FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString() ?? string.Empty,
                Action = e.State.ToString(),
                OldValues = e.State == EntityState.Added
                    ? null
                    : JsonSerializer.Serialize(e.OriginalValues.ToObject()),
                NewValues = e.State == EntityState.Deleted
                    ? null
                    : JsonSerializer.Serialize(e.CurrentValues.ToObject()),
                UserId = userId,
                CorrelationId = correlationId,
                Timestamp = DateTime.UtcNow
            })
            .ToList();

        if (auditEntries.Count > 0)
            context.Set<AuditLog>().AddRange(auditEntries);
    }
}
