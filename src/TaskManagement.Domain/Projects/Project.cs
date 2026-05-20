using TaskManagement.Domain.Common.Base;
using TaskManagement.Domain.Common.Errors;
using TaskManagement.Domain.Common.Markers;
using TaskManagement.Domain.Projects.Events;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Domain.Projects;

public sealed class Project : AggregateRoot<ProjectId>, ISoftDeletable
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public ProjectStatus Status { get; private set; }
    public string OwnerId { get; private set; } = string.Empty;
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private readonly List<ProjectTask> _tasks = [];
    public IReadOnlyCollection<ProjectTask> Tasks => _tasks.AsReadOnly();

    private Project() { }

    public static Result<Project> Create(string name, string? description, string ownerId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ProjectErrors.NameRequired;

        var project = new Project
        {
            Id = ProjectId.New(),
            Name = name.Trim(),
            Description = description?.Trim(),
            Status = ProjectStatus.Active,
            OwnerId = ownerId
        };

        //project.RaiseDomainEvent(new ProjectCreatedDomainEvent(project.Id));
        return project;
    }

    public Result Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ProjectErrors.NameRequired;

        Name = name.Trim();
        Description = description?.Trim();
        return Result.Success();
    }

    public Result UpdateStatus(ProjectStatus status)
    {
        Status = status;
        return Result.Success();
    }

    public Result Delete()
    {
        if (IsDeleted) return ProjectErrors.AlreadyDeleted;
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        //RaiseDomainEvent(new ProjectDeletedDomainEvent(Id));
        return Result.Success();
    }
}
