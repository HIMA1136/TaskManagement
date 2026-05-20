using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Projects;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Infrastructure.Persistence.Repositories;

public sealed class TaskRepository(ApplicationDbContext context) : ITaskRepository
{
    public async Task<ProjectTask?> GetByIdAsync(TaskId id, CancellationToken cancellationToken = default) =>
        await context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ProjectTask>> GetByProjectIdAsync(
        ProjectId projectId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default) =>
        await context.Tasks
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    public async Task<int> CountByProjectIdAsync(ProjectId projectId, CancellationToken cancellationToken = default) =>
        await context.Tasks.CountAsync(t => t.ProjectId == projectId, cancellationToken);

    public async Task AddAsync(ProjectTask task, CancellationToken cancellationToken = default) =>
        await context.Tasks.AddAsync(task, cancellationToken);

    public void Update(ProjectTask task) =>
        context.Tasks.Update(task);

    public void Remove(ProjectTask task) =>
        context.Tasks.Remove(task);
}
