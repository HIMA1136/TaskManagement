using TaskManagement.Domain.Common.Markers;

namespace TaskManagement.Domain.Common.Base;

public abstract record DomainEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
