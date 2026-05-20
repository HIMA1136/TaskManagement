using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Projects;

namespace TaskManagement.Infrastructure.Persistence.Repositories;

public sealed class ProjectRepository(ApplicationDbContext context) : IProjectRepository
{
    public async Task<Project?> GetByIdAsync(ProjectId id, CancellationToken cancellationToken = default) =>
        await context.Projects
            .Include(p => p.Tasks)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Project>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default) =>
        await context.Projects
            .Include(p => p.Tasks)
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        await context.Projects.CountAsync(cancellationToken);

    public async Task AddAsync(Project project, CancellationToken cancellationToken = default) =>
        await context.Projects.AddAsync(project, cancellationToken);

    public void Update(Project project) =>
        context.Projects.Update(project);

    public void Remove(Project project) =>
        context.Projects.Remove(project);
}
