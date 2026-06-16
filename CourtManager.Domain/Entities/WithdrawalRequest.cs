using CourtManager.Domain.Enums;

namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a withdrawal request from an owner.
/// </summary>
public class WithdrawalRequest
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public decimal Amount { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string BankAccountNumber { get; set; } = string.Empty;
    public string BankAccountHolderName { get; set; } = string.Empty;
    public WithdrawalStatus Status { get; set; } = WithdrawalStatus.Pending;
    public Guid? ApprovedByAdminId { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }

    // Navigation properties
    public User? Owner { get; set; }
    public User? ApprovedByAdmin { get; set; }
    public ICollection<WalletTransaction> WalletTransactions { get; set; } = [];
}
