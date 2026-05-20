using TaskManagement.Domain.Common.Errors;

namespace TaskManagement.Domain.Tasks;

public static class TaskErrors
{
    public static readonly Error TitleRequired =
        Error.Validation("Task.TitleRequired", "Task title is required.");

    public static readonly Error AlreadyDeleted =
        Error.Conflict("Task.AlreadyDeleted", "The task has already been deleted.");

    public static Error NotFound(Guid id) =>
        Error.NotFound("Task.NotFound", $"Task with ID '{id}' was not found.");

    public static Error NotBelongToProject(Guid taskId, Guid projectId) =>
        Error.Conflict("Task.NotBelongToProject", $"Task '{taskId}' does not belong to project '{projectId}'.");
}
