using TaskManagement.Application.Common.Abstractions;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Common.Errors;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Application.Features.Tasks.Commands.DeleteTask;

public sealed class DeleteTaskCommandHandler(
    ITaskRepository taskRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteTaskCommand>
{
    public async Task<Result> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetByIdAsync(new TaskId(request.Id), cancellationToken);
        if (task is null) return TaskErrors.NotFound(request.Id);

        if (task.ProjectId.Value != request.ProjectId)
            return TaskErrors.NotBelongToProject(request.Id, request.ProjectId);

        var result = task.Delete();
        if (result.IsFailure) return result;

        taskRepository.Update(task);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
