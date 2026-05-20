using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Projects;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

public sealed class TaskConfiguration : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasConversion(id => id.Value, value => new TaskId(value))
            .ValueGeneratedNever();

        builder.Property(t => t.ProjectId)
            .HasConversion(id => id.Value, value => new ProjectId(value));

        builder.Property(t => t.Title)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(4000);

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Priority)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.AssigneeId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(t => t.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasQueryFilter(t => !t.IsDeleted);

        builder.ToTable("Tasks");
    }
}
