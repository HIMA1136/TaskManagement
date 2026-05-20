using TaskManagement.Application.Common.Abstractions;
using TaskManagement.Application.Features.Tasks.DTOs;
using TaskManagement.Domain.Common.Errors;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTaskById;

public sealed class GetTaskByIdQueryHandler(ITaskRepository taskRepository)
    : IQueryHandler<GetTaskByIdQuery, TaskDto>
{
    public async Task<Result<TaskDto>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetByIdAsync(new TaskId(request.Id), cancellationToken);
        if (task is null) return TaskErrors.NotFound(request.Id);

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
