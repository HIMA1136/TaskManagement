namespace TaskManagement.Domain.Projects;

public readonly record struct ProjectId(Guid Value)
{
    public static ProjectId New() => new(Guid.NewGuid());
    public static ProjectId Empty => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}
