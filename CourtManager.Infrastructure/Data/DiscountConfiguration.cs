using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;

namespace CourtManager.Infrastructure.Data;

public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.HasKey(d => d.DiscountId);
        builder.Property(d => d.Code).IsRequired().HasMaxLength(50);
        builder.Property(d => d.Name).IsRequired().HasMaxLength(200);
        builder.Property(d => d.DiscountType).IsRequired().HasConversion<string>().HasMaxLength(50);
        builder.Property(d => d.Value).HasPrecision(10, 2);
        builder.Property(d => d.MinBookingAmount).HasPrecision(10, 2);
        builder.Property(d => d.MaxDiscountAmount).HasPrecision(10, 2);
        builder.Property(d => d.CreatedAt).ValueGeneratedOnAdd().HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.HasQueryFilter(d => !d.IsDeleted);
        builder.HasIndex(d => d.Code).IsUnique();

        builder.HasOne(d => d.Owner)
            .WithMany(u => u.CreatedDiscounts)
            .HasForeignKey(d => d.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Venue)
            .WithMany(v => v.Discounts)
            .HasForeignKey(d => d.VenueId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Discounts");
    }
}
