using CourtManager.Domain.Enums;

namespace CourtManager.Application.DTOs;

/// <summary>
/// Data Transfer Object for WalletTransaction.
/// </summary>
public class WalletTransactionDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? RelatedBookingId { get; set; }
    public Guid? RelatedWithdrawalId { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Response for wallet summary.
/// </summary>
public class WalletSummaryDto
{
    public decimal Balance { get; set; }
    public decimal PendingWithdrawal { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalWithdrawn { get; set; }
    public int TransactionCount { get; set; }
}

/// <summary>
/// Response for wallet history with pagination.
/// </summary>
public class WalletHistoryDto
{
    public List<WalletTransactionDto> Transactions { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

/// <summary>
/// Response for owner wallet info.
/// </summary>
public class OwnerWalletDto
{
    public decimal Balance { get; set; }
    public int PendingWithdrawalCount { get; set; }
    public decimal PendingWithdrawalAmount { get; set; }
}
