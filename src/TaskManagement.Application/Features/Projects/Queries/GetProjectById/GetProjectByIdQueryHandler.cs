using TaskManagement.Application.Common.Abstractions;
using TaskManagement.Application.Features.Projects.DTOs;
using TaskManagement.Application.Features.Tasks.DTOs;
using TaskManagement.Domain.Common.Errors;
using TaskManagement.Domain.Projects;

namespace TaskManagement.Application.Features.Projects.Queries.GetProjectById;

public sealed class GetProjectByIdQueryHandler(IProjectRepository projectRepository)
    : IQueryHandler<GetProjectByIdQuery, ProjectDto>
{
    public async Task<Result<ProjectDto>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetByIdAsync(new ProjectId(request.Id), cancellationToken);
        if (project is null) return ProjectErrors.NotFound(request.Id);

        return new ProjectDto(
            project.Id.Value,
            project.Name,
            project.Description,
            project.Status.ToString(),
            project.OwnerId,
            project.CreatedAt,
            project.UpdatedAt,
            project.Tasks.Select(t => new TaskSummaryDto(
                t.Id.Value,
                t.Title,
                t.Status.ToString(),
                t.Priority.ToString(),
                t.DueDate,
                t.AssigneeId)).ToList());
    }
}
