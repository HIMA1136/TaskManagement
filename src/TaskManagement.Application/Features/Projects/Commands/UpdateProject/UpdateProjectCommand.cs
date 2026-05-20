using TaskManagement.Application.Common.Abstractions;

namespace TaskManagement.Application.Features.Projects.Commands.UpdateProject;

public sealed record UpdateProjectCommand(
    Guid Id,
    string Name,
    string? Description) : ICommand;
