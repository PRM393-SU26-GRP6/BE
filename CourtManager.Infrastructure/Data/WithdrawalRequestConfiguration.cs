using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;

namespace CourtManager.Infrastructure.Data;

public class WithdrawalRequestConfiguration : IEntityTypeConfiguration<WithdrawalRequest>
{
    public void Configure(EntityTypeBuilder<WithdrawalRequest> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id).ValueGeneratedNever();
        builder.Property(w => w.OwnerId).IsRequired();
        builder.Property(w => w.Amount).HasPrecision(10, 2).IsRequired();
        builder.Property(w => w.BankName).HasMaxLength(100).IsRequired();
        builder.Property(w => w.BankAccountNumber).HasMaxLength(50).IsRequired();
        builder.Property(w => w.BankAccountHolderName).HasMaxLength(200).IsRequired();
        builder.Property(w => w.Status).IsRequired().HasConversion<string>().HasDefaultValue(WithdrawalStatus.Pending).HasMaxLength(50);
        builder.Property(w => w.RejectionReason).HasMaxLength(500);
        builder.Property(w => w.CreatedAt).ValueGeneratedOnAdd().HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        builder.HasOne(w => w.Owner).WithMany(u => u.WithdrawalRequests).HasForeignKey(w => w.OwnerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(w => w.ApprovedByAdmin).WithMany().HasForeignKey(w => w.ApprovedByAdminId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(w => w.WalletTransactions).WithOne(wt => wt.RelatedWithdrawal).HasForeignKey(wt => wt.RelatedWithdrawalId).OnDelete(DeleteBehavior.SetNull);
        
        builder.HasIndex(w => w.OwnerId);
        builder.HasIndex(w => w.Status);
        builder.HasIndex(w => w.CreatedAt);
        builder.ToTable("WithdrawalRequests");
    }
}
