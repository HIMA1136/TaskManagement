namespace TaskManagement.Application.Features.Tasks.DTOs;

public sealed record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    string Priority,
    DateTime? DueDate,
    string AssigneeId,
    Guid ProjectId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
