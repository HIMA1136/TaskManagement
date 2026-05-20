using TaskManagement.Domain.Common.Errors;

namespace TaskManagement.Domain.Projects;

public static class ProjectErrors
{
    public static readonly Error NameRequired =
        Error.Validation("Project.NameRequired", "Project name is required.");

    public static readonly Error AlreadyDeleted =
        Error.Conflict("Project.AlreadyDeleted", "The project has already been deleted.");

    public static Error NotFound(Guid id) =>
        Error.NotFound("Project.NotFound", $"Project with ID '{id}' was not found.");

    public static Error NotOwnedByUser(Guid id) =>
        Error.Unauthorized("Project.NotOwnedByUser", $"You do not have access to project '{id}'.");
}
