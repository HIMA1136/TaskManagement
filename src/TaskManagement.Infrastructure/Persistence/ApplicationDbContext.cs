using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Projects;
using TaskManagement.Domain.Tasks;
using TaskManagement.Infrastructure.Identity;
using TaskManagement.Infrastructure.Persistence.Entities;
using TaskManagement.Infrastructure.Persistence.Interceptors;

namespace TaskManagement.Infrastructure.Persistence;

public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    private readonly AuditSaveChangesInterceptor _auditInterceptor;
    private readonly DomainEventDispatchInterceptor _domainEventInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        AuditSaveChangesInterceptor auditInterceptor,
        DomainEventDispatchInterceptor domainEventInterceptor)
        : base(options)
    {
        _auditInterceptor = auditInterceptor;
        _domainEventInterceptor = domainEventInterceptor;
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectTask> Tasks => Set<ProjectTask>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<IdempotencyKey> IdempotencyKeys => Set<IdempotencyKey>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditInterceptor, _domainEventInterceptor);
    }
}
