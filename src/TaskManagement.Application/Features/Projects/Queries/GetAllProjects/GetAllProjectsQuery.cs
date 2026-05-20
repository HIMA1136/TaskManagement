using TaskManagement.Application.Common.Abstractions;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Features.Projects.DTOs;

namespace TaskManagement.Application.Features.Projects.Queries.GetAllProjects;

public sealed record GetAllProjectsQuery(int Page = 1, int PageSize = 20)
    : ICachedQuery<PagedResult<ProjectSummaryDto>>
{
    public string CacheKey => $"projects:all:page={Page}:size={PageSize}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}
