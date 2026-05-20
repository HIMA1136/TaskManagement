using TaskManagement.Application.Common.Abstractions;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Features.Tasks.DTOs;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTasksByProject;

public sealed record GetTasksByProjectQuery(Guid ProjectId, int Page = 1, int PageSize = 20)
    : ICachedQuery<PagedResult<TaskSummaryDto>>
{
    public string CacheKey => $"tasks:project={ProjectId}:page={Page}:size={PageSize}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(3);
}
