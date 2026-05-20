using TaskManagement.Application.Common.Abstractions;

namespace TaskManagement.Application.Features.Tasks.Commands.DeleteTask;

public sealed record DeleteTaskCommand(Guid Id, Guid ProjectId) : ICommand;
