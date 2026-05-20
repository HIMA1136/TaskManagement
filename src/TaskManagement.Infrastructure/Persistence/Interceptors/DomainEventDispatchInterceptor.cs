using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TaskManagement.Domain.Common.Base;
using TaskManagement.Domain.Common.Markers;
using TaskManagement.Domain.Projects;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Infrastructure.Persistence.Interceptors;

public sealed class DomainEventDispatchInterceptor(IPublisher publisher) : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync(eventData.Context, cancellationToken);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        DispatchDomainEventsAsync(eventData.Context, CancellationToken.None).GetAwaiter().GetResult();
        return base.SavedChanges(eventData, result);
    }

    private async Task DispatchDomainEventsAsync(DbContext? context, CancellationToken cancellationToken)
    {
        if (context is null) return;

        var projectAggregates = context.ChangeTracker
            .Entries<AggregateRoot<ProjectId>>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .ToList();

        var taskEntities = context.ChangeTracker
            .Entries<ProjectTask>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .ToList();

        var allEvents = projectAggregates
            .SelectMany(e => e.DomainEvents)
            .Concat(taskEntities.SelectMany(e => e.DomainEvents))
            .ToList();

        projectAggregates.ForEach(e => e.ClearDomainEvents());
        taskEntities.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in allEvents)
        {
            await publisher.Publish(domainEvent, cancellationToken);
        }
    }
}
