using TaskManagement.Application.Common.Abstractions;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Features.Tasks.DTOs;
using TaskManagement.Domain.Common.Errors;
using TaskManagement.Domain.Projects;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Application.Features.Tasks.Commands.CreateTask;

public sealed class CreateTaskCommandHandler(
    IProjectRepository projectRepository,
    ITaskRepository taskRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateTaskCommand, TaskDto>
{
    public async Task<Result<TaskDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var projectId = new ProjectId(request.ProjectId);
        var project = await projectRepository.GetByIdAsync(projectId, cancellationToken);
        if (project is null) return ProjectErrors.NotFound(request.ProjectId);

        var result = ProjectTask.Create(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            request.AssigneeId,
            projectId);

        if (result.IsFailure) return result.Error!;

        var task = result.Value!;
        await taskRepository.AddAsync(task, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new TaskDto(
            task.Id.Value,
            task.Title,
            task.Description,
            task.Status.ToString(),
            task.Priority.ToString(),
            task.DueDate,
            task.AssigneeId,
            task.ProjectId.Value,
            task.CreatedAt,
            task.UpdatedAt);
    }
}
