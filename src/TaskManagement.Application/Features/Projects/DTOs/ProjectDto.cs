using TaskManagement.Application.Features.Tasks.DTOs;

namespace TaskManagement.Application.Features.Projects.DTOs;

public sealed record ProjectDto(
    Guid Id,
    string Name,
    string? Description,
    string Status,
    string OwnerId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyList<TaskSummaryDto> Tasks);
