using CourtManager.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourtManager.Infrastructure.Data;

/// <summary>
/// EF configuration for ApplicationUser (Identity auth entity).
/// Maps to the "AspNetUsers" table, separate from the Domain "Users" table.
/// </summary>
public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("AspNetUsers");
    }
}
