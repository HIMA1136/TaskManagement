namespace TaskManagement.Application.Features.Projects.DTOs;

public sealed record ProjectSummaryDto(
    Guid Id,
    string Name,
    string? Description,
    string Status,
    string OwnerId,
    DateTime CreatedAt,
    int TaskCount);
