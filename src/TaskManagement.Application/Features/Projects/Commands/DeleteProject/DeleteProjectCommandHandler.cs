using TaskManagement.Application.Common.Abstractions;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Common.Errors;
using TaskManagement.Domain.Projects;

namespace TaskManagement.Application.Features.Projects.Commands.DeleteProject;

public sealed class DeleteProjectCommandHandler(
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : ICommandHandler<DeleteProjectCommand>
{
    public async Task<Result> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetByIdAsync(new ProjectId(request.Id), cancellationToken);
        if (project is null) return ProjectErrors.NotFound(request.Id);

        var userId = currentUserService.UserId ?? string.Empty;
        if (project.OwnerId != userId) return ProjectErrors.NotOwnedByUser(request.Id);

        var result = project.Delete();
        if (result.IsFailure) return result;

        projectRepository.Update(project);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
