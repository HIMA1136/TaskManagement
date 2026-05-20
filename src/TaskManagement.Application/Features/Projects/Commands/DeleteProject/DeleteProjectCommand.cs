using TaskManagement.Application.Common.Abstractions;

namespace TaskManagement.Application.Features.Projects.Commands.DeleteProject;

public sealed record DeleteProjectCommand(Guid Id) : ICommand;
