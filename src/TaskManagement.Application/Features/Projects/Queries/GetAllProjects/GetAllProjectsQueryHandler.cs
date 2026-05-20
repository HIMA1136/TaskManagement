using TaskManagement.Application.Common.Abstractions;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Features.Projects.DTOs;
using TaskManagement.Domain.Common.Errors;
using TaskManagement.Domain.Projects;

namespace TaskManagement.Application.Features.Projects.Queries.GetAllProjects;

public sealed class GetAllProjectsQueryHandler(IProjectRepository projectRepository)
    : IQueryHandler<GetAllProjectsQuery, PagedResult<ProjectSummaryDto>>
{
    public async Task<Result<PagedResult<ProjectSummaryDto>>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var projects = await projectRepository.GetAllAsync(page, pageSize, cancellationToken);
        var totalCount = await projectRepository.CountAsync(cancellationToken);

        var dtos = projects.Select(p => new ProjectSummaryDto(
            p.Id.Value,
            p.Name,
            p.Description,
            p.Status.ToString(),
            p.OwnerId,
            p.CreatedAt,
            p.Tasks.Count)).ToList();

        return PagedResult<ProjectSummaryDto>.Create(dtos, totalCount, page, pageSize);
    }
}
