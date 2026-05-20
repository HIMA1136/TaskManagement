using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Projects;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Project> Projects { get; }
    DbSet<ProjectTask> Tasks { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
