using TaskManagement.Domain.Common.Markers;

namespace TaskManagement.Domain.Common.Base;

public abstract class ValueObject : IValueObject
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType()) return false;
        return ((ValueObject)obj).GetEqualityComponents().SequenceEqual(GetEqualityComponents());
    }

    public override int GetHashCode() =>
        GetEqualityComponents()
            .Aggregate(default(int), (current, obj) =>
                HashCode.Combine(current, obj?.GetHashCode() ?? 0));

    public static bool operator ==(ValueObject? left, ValueObject? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);
}
