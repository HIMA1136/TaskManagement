using TaskManagement.Application.Common.Abstractions;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTask;

public sealed record UpdateTaskCommand(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate) : ICommand;
