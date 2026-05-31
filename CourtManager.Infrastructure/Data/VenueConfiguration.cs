using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;

namespace CourtManager.Infrastructure.Data;

/// <summary>
/// Entity configuration for Venue entity using Fluent API.
/// </summary>
public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        // Primary Key
        builder.HasKey(v => v.VenueId);

        // Properties
        builder.Property(v => v.VenueName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(v => v.Latitude)
            .HasPrecision(18, 10);

        builder.Property(v => v.Longitude)
            .HasPrecision(18, 10);

        builder.Property(v => v.PhoneContact)
            .HasMaxLength(20);

        builder.Property(v => v.OpeningHours)
            .HasMaxLength(500);

        // Soft delete query filter
        builder.HasQueryFilter(v => !v.IsDeleted);

        // Relationships
        builder.HasOne(v => v.Owner)
            .WithMany(u => u.OwnedVenues)
            .HasForeignKey(v => v.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(v => v.FootballFields)
            .WithOne(f => f.Venue)
            .HasForeignKey(f => f.VenueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.VenueImages)
            .WithOne(i => i.Venue)
            .HasForeignKey(i => i.VenueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.Reviews)
            .WithOne(r => r.Venue)
            .HasForeignKey(r => r.VenueId)
            .OnDelete(DeleteBehavior.Cascade);

        // Table configuration
        builder.ToTable("Venues");
    }
}
