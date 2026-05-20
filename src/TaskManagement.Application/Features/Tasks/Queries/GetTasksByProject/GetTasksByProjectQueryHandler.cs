using TaskManagement.Application.Common.Abstractions;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Features.Tasks.DTOs;
using TaskManagement.Domain.Common.Errors;
using TaskManagement.Domain.Projects;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTasksByProject;

public sealed class GetTasksByProjectQueryHandler(ITaskRepository taskRepository)
    : IQueryHandler<GetTasksByProjectQuery, PagedResult<TaskSummaryDto>>
{
    public async Task<Result<PagedResult<TaskSummaryDto>>> Handle(GetTasksByProjectQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var projectId = new ProjectId(request.ProjectId);

        var tasks = await taskRepository.GetByProjectIdAsync(projectId, page, pageSize, cancellationToken);
        var totalCount = await taskRepository.CountByProjectIdAsync(projectId, cancellationToken);

        var dtos = tasks.Select(t => new TaskSummaryDto(
            t.Id.Value,
            t.Title,
            t.Status.ToString(),
            t.Priority.ToString(),
            t.DueDate,
            t.AssigneeId)).ToList();

        return PagedResult<TaskSummaryDto>.Create(dtos, totalCount, page, pageSize);
    }
}
