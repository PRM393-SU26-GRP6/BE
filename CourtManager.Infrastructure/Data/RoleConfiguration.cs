using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;

namespace CourtManager.Infrastructure.Data;

/// <summary>
/// Entity configuration for Role entity.
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Seed default roles
        builder.HasData(
            new Role
            {
                Id = new Guid("10000000-0000-0000-0000-000000000001"),
                Name = "Admin",
                NormalizedName = "ADMIN",
                Description = "Administrator with full access",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ConcurrencyStamp = "10000000-0000-0000-0000-000000000001"
            },
            new Role
            {
                Id = new Guid("10000000-0000-0000-0000-000000000002"),
                Name = "Owner",
                NormalizedName = "OWNER",
                Description = "Court owner",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ConcurrencyStamp = "10000000-0000-0000-0000-000000000002"
            },
            new Role
            {
                Id = new Guid("10000000-0000-0000-0000-000000000003"),
                Name = "User",
                NormalizedName = "USER",
                Description = "Regular user",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ConcurrencyStamp = "10000000-0000-0000-0000-000000000003"
            }
        );
    }
}
