namespace TaskManagement.Blazor.Models;

public sealed record LoginRequest(string Email, string Password);

public sealed record AuthResponse(
    string UserId,
    string Email,
    string Token,
    DateTime ExpiresAt);

public sealed record ProjectSummaryDto(
    Guid Id,
    string Name,
    string? Description,
    string Status,
    string OwnerId,
    DateTime CreatedAt,
    int TaskCount);

public sealed record TaskSummaryDto(
    Guid Id,
    string Title,
    string Status,
    string Priority,
    DateTime? DueDate,
    string AssigneeId);

public sealed record ProjectDto(
    Guid Id,
    string Name,
    string? Description,
    string Status,
    string OwnerId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyList<TaskSummaryDto> Tasks);

public sealed record CreateProjectRequest(string Name, string? Description);

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    string Priority,
    DateTime? DueDate,
    string AssigneeId);

public sealed record ChangeTaskStatusRequest(string NewStatus);

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public bool HasNextPage { get; init; }
    public bool HasPreviousPage { get; init; }
}
