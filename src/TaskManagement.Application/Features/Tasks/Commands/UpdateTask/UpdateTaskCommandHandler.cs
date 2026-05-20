using TaskManagement.Application.Common.Abstractions;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Common.Errors;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTask;

public sealed class UpdateTaskCommandHandler(
    ITaskRepository taskRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateTaskCommand>
{
    public async Task<Result> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetByIdAsync(new TaskId(request.Id), cancellationToken);
        if (task is null) return TaskErrors.NotFound(request.Id);

        if (task.ProjectId.Value != request.ProjectId)
            return TaskErrors.NotBelongToProject(request.Id, request.ProjectId);

        var result = task.Update(request.Title, request.Description, request.Priority, request.DueDate);
        if (result.IsFailure) return result;

        taskRepository.Update(task);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
