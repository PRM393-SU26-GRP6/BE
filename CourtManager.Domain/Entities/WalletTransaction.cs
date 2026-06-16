using CourtManager.Domain.Enums;

namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a wallet transaction for audit logging.
/// </summary>
public class WalletTransaction
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public WalletTransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? RelatedBookingId { get; set; }
    public Guid? RelatedWithdrawalId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public User? Owner { get; set; }
    public Booking? RelatedBooking { get; set; }
    public WithdrawalRequest? RelatedWithdrawal { get; set; }
}
