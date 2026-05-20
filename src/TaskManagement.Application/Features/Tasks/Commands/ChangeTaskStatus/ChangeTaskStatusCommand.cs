using TaskManagement.Application.Common.Abstractions;

namespace TaskManagement.Application.Features.Tasks.Commands.ChangeTaskStatus;

public sealed record ChangeTaskStatusCommand(
    Guid Id,
    Guid ProjectId,
    Domain.Tasks.TaskStatus NewStatus) : ICommand;
