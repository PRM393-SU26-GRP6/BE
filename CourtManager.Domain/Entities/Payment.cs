using CourtManager.Domain.Enums;

namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a payment transaction for one or more bookings.
/// </summary>
public class Payment
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public PaymentType PaymentType { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string TransactionCode { get; set; } = string.Empty;
    public string? Gateway { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? GatewayReferenceCode { get; set; }
    public string? GatewayAccountNumber { get; set; }
    public string? GatewayRawContent { get; set; }
    public string? GatewayDescription { get; set; }
    public string? TransactionDate { get; set; } // Raw date string from gateway
    public DateTime? PaidAt { get; set; }
    public decimal? RefundAmount { get; set; } // Set when PaymentType = Refund
    public string? RefundReason { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navigation property
    public Booking? Booking { get; set; }
}
