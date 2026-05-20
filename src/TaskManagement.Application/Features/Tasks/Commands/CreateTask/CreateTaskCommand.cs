using TaskManagement.Application.Common.Abstractions;
using TaskManagement.Application.Features.Tasks.DTOs;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Application.Features.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(
    Guid ProjectId,
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate,
    string AssigneeId) : ICommand<TaskDto>;
