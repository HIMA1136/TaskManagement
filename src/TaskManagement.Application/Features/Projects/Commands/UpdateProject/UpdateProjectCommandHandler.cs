using TaskManagement.Application.Common.Abstractions;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Common.Errors;
using TaskManagement.Domain.Projects;

namespace TaskManagement.Application.Features.Projects.Commands.UpdateProject;

public sealed class UpdateProjectCommandHandler(
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : ICommandHandler<UpdateProjectCommand>
{
    public async Task<Result> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetByIdAsync(new ProjectId(request.Id), cancellationToken);
        if (project is null) return ProjectErrors.NotFound(request.Id);

        var userId = currentUserService.UserId ?? string.Empty;
        if (project.OwnerId != userId) return ProjectErrors.NotOwnedByUser(request.Id);

        var result = project.Update(request.Name, request.Description);
        if (result.IsFailure) return result;

        projectRepository.Update(project);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
