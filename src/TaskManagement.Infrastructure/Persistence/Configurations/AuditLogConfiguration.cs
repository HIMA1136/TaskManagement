using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Infrastructure.Persistence.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.EntityName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.EntityId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(a => a.Action)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.OldValues)
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.NewValues)
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.UserId)
            .HasMaxLength(450);

        builder.Property(a => a.CorrelationId)
            .HasMaxLength(100);

        builder.ToTable("AuditLogs");
    }
}
