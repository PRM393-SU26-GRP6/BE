using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;

namespace CourtManager.Infrastructure.Data;

public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
{
    public void Configure(EntityTypeBuilder<WalletTransaction> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id).ValueGeneratedNever();
        builder.Property(w => w.OwnerId).IsRequired();
        builder.Property(w => w.Amount).HasPrecision(10, 2).IsRequired();
        builder.Property(w => w.Description).HasMaxLength(500).IsRequired();
        builder.Property(w => w.Type).IsRequired().HasConversion<string>().HasMaxLength(50);
        builder.Property(w => w.CreatedAt).ValueGeneratedOnAdd().HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        builder.HasOne(w => w.Owner).WithMany(u => u.WalletTransactions).HasForeignKey(w => w.OwnerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(w => w.RelatedBooking).WithMany().HasForeignKey(w => w.RelatedBookingId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(w => w.RelatedWithdrawal).WithMany(wr => wr.WalletTransactions).HasForeignKey(w => w.RelatedWithdrawalId).OnDelete(DeleteBehavior.SetNull);
        
        builder.HasIndex(w => w.OwnerId);
        builder.HasIndex(w => w.Type);
        builder.HasIndex(w => w.CreatedAt);
        builder.ToTable("WalletTransactions");
    }
}
