using TaskManagement.Domain.Common.Base;
using TaskManagement.Domain.Common.Errors;
using TaskManagement.Domain.Common.Markers;
using TaskManagement.Domain.Projects;
using TaskManagement.Domain.Tasks.Events;

namespace TaskManagement.Domain.Tasks;

public sealed class ProjectTask : Entity<TaskId>, ISoftDeletable
{
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TaskStatus Status { get; private set; }
    public TaskPriority Priority { get; private set; }
    public DateTime? DueDate { get; private set; }
    public string AssigneeId { get; private set; } = string.Empty;
    public ProjectId ProjectId { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public void ClearDomainEvents() => _domainEvents.Clear();

    private void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    private ProjectTask() { }

    public static Result<ProjectTask> Create(
        string title,
        string? description,
        TaskPriority priority,
        DateTime? dueDate,
        string assigneeId,
        ProjectId projectId)
    {
        if (string.IsNullOrWhiteSpace(title))
            return TaskErrors.TitleRequired;

        var task = new ProjectTask
        {
            Id = TaskId.New(),
            Title = title.Trim(),
            Description = description?.Trim(),
            Status = TaskStatus.Todo,
            Priority = priority,
            DueDate = dueDate,
            AssigneeId = assigneeId,
            ProjectId = projectId
        };

        //task.RaiseDomainEvent(new TaskCreatedDomainEvent(task.Id, projectId));
        return task;
    }

    public Result ChangeStatus(TaskStatus newStatus)
    {
        if (Status == newStatus) return Result.Success();
        var old = Status;
        Status = newStatus;
        //RaiseDomainEvent(new TaskStatusChangedDomainEvent(Id, old, newStatus));
        return Result.Success();
    }

    public Result Update(string title, string? description, TaskPriority priority, DateTime? dueDate)
    {
        if (string.IsNullOrWhiteSpace(title))
            return TaskErrors.TitleRequired;

        Title = title.Trim();
        Description = description?.Trim();
        Priority = priority;
        DueDate = dueDate;
        return Result.Success();
    }

    public Result Delete()
    {
        if (IsDeleted) return TaskErrors.AlreadyDeleted;
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        return Result.Success();
    }
}
