using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;

namespace CourtManager.Infrastructure.Data;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(p => p.BookingId).IsRequired();
        builder.Property(p => p.Amount).HasPrecision(10, 2);
        builder.Property(p => p.RefundAmount).HasPrecision(10, 2);
        builder.Property(p => p.RefundReason).HasMaxLength(500);
        builder.Property(p => p.PaymentType).IsRequired().HasConversion<string>().HasMaxLength(50);
        builder.Property(p => p.PaymentMethod).HasConversion<int>();
        builder.Property(p => p.PaymentStatus).IsRequired().HasConversion<string>().HasDefaultValue(CourtManager.Domain.Enums.PaymentStatus.Pending).HasMaxLength(50);
        builder.Property(p => p.TransactionCode).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Gateway).HasMaxLength(50);
        builder.Property(p => p.GatewayTransactionId).HasMaxLength(100);
        builder.Property(p => p.GatewayReferenceCode).HasMaxLength(100);
        builder.Property(p => p.GatewayAccountNumber).HasMaxLength(100);
        builder.Property(p => p.GatewayRawContent).HasMaxLength(500);
        builder.Property(p => p.PaidAt).IsRequired(false);
        builder.HasQueryFilter(p => !p.IsDeleted);
        builder.HasIndex(p => p.BookingId);
        builder.HasIndex(p => p.TransactionCode).IsUnique();
        builder.HasIndex(p => new { p.Gateway, p.GatewayReferenceCode }).IsUnique();
        builder.HasIndex(p => new { p.Gateway, p.GatewayTransactionId }).IsUnique();
        builder.HasOne(p => p.Booking).WithMany(b => b.Payments).HasForeignKey(p => p.BookingId).OnDelete(DeleteBehavior.Cascade);
        builder.ToTable("Payments");
    }
}
