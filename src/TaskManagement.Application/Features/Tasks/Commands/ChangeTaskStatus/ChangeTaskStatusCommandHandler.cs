using TaskManagement.Application.Common.Abstractions;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Common.Errors;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Application.Features.Tasks.Commands.ChangeTaskStatus;

public sealed class ChangeTaskStatusCommandHandler(
    ITaskRepository taskRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<ChangeTaskStatusCommand>
{
    public async Task<Result> Handle(ChangeTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetByIdAsync(new TaskId(request.Id), cancellationToken);
        if (task is null) return TaskErrors.NotFound(request.Id);

        if (task.ProjectId.Value != request.ProjectId)
            return TaskErrors.NotBelongToProject(request.Id, request.ProjectId);

        var result = task.ChangeStatus(request.NewStatus);
        if (result.IsFailure) return result;

        taskRepository.Update(task);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
