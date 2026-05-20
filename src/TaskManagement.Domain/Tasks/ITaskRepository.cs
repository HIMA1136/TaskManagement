using TaskManagement.Domain.Projects;

namespace TaskManagement.Domain.Tasks;

public interface ITaskRepository
{
    Task<ProjectTask?> GetByIdAsync(TaskId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProjectTask>> GetByProjectIdAsync(ProjectId projectId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountByProjectIdAsync(ProjectId projectId, CancellationToken cancellationToken = default);
    Task AddAsync(ProjectTask task, CancellationToken cancellationToken = default);
    void Update(ProjectTask task);
    void Remove(ProjectTask task);
}
