namespace TaskManagement.Domain.Projects;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(ProjectId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Project>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Project project, CancellationToken cancellationToken = default);
    void Update(Project project);
    void Remove(Project project);
}
