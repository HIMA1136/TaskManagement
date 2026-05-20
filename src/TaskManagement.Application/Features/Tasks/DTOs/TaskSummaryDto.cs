namespace TaskManagement.Application.Features.Tasks.DTOs;

public sealed record TaskSummaryDto(
    Guid Id,
    string Title,
    string Status,
    string Priority,
    DateTime? DueDate,
    string AssigneeId);
