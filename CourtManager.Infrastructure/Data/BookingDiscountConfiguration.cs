using CourtManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourtManager.Infrastructure.Data;

public class BookingDiscountConfiguration : IEntityTypeConfiguration<BookingDiscount>
{
    public void Configure(EntityTypeBuilder<BookingDiscount> builder)
    {
        builder.HasKey(bd => new { bd.BookingId, bd.DiscountId });

        builder.Property(bd => bd.DiscountAmount)
            .HasPrecision(10, 2)
            .IsRequired();

        // Soft delete query filter
        builder.HasQueryFilter(bd => !bd.IsDeleted);

        builder.HasOne(bd => bd.Booking)
            .WithMany(b => b.BookingDiscounts)
            .HasForeignKey(bd => bd.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bd => bd.Discount)
            .WithMany(d => d.BookingDiscounts)
            .HasForeignKey(bd => bd.DiscountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
