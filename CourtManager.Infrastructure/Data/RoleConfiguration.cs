using CourtManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourtManager.Infrastructure.Data;

/// <summary>
/// Entity configuration for Domain Role POCO.
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.Property(r => r.Name).HasMaxLength(256).IsRequired();
        builder.Property(r => r.NormalizedName).HasMaxLength(256).IsRequired(false);

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Seed default roles (same IDs as before)
        builder.HasData(
            new Role
            {
                Id = new Guid("b5abbaf1-931c-5353-b9ab-1f38eb30b8b8"),
                Name = "Admin",
                NormalizedName = "ADMIN",
                Description = "Administrator with full access",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Role
            {
                Id = new Guid("07371171-eec1-3255-b1b2-1d8e8e81ede7"),
                Name = "Owner",
                NormalizedName = "OWNER",
                Description = "Venue and field owner",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Role
            {
                Id = new Guid("76075424-3dac-6259-a0f7-00a4c6c20191"),
                Name = "User",
                NormalizedName = "USER",
                Description = "Regular booking user",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
