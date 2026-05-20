using TaskManagement.Application.Common.Abstractions;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Features.Projects.DTOs;
using TaskManagement.Domain.Common.Errors;
using TaskManagement.Domain.Projects;

namespace TaskManagement.Application.Features.Projects.Commands.CreateProject;

public sealed class CreateProjectCommandHandler(
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : ICommandHandler<CreateProjectCommand, ProjectSummaryDto>
{
    public async Task<Result<ProjectSummaryDto>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = currentUserService.UserId ?? string.Empty;

            var result = Project.Create(request.Name, request.Description, userId);
            if (result.IsFailure) return result.Error!;

            var project = result.Value!;
            await projectRepository.AddAsync(project, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new ProjectSummaryDto(
                project.Id.Value,
                project.Name,
                project.Description,
                project.Status.ToString(),
                project.OwnerId,
                project.CreatedAt,
                0);
        }
        catch(Exception ex) {
            throw; }
    }
}
