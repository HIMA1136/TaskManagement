using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Infrastructure.Persistence.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

public sealed class IdempotencyKeyConfiguration : IEntityTypeConfiguration<IdempotencyKey>
{
    public void Configure(EntityTypeBuilder<IdempotencyKey> builder)
    {
        builder.HasKey(i => i.Key);

        builder.Property(i => i.RequestName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(i => i.Response)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.ToTable("IdempotencyKeys");
    }
}
