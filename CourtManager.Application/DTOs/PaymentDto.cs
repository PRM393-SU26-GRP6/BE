using System;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;

namespace CourtManager.Application.DTOs;

/// <summary>
/// Data Transfer Object for Payment.
/// </summary>
public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string PaymentType { get; set; } = string.Empty;
    public PaymentMethod PaymentMethod { get; set; }
    public string TransactionCode { get; set; } = string.Empty;
    public DateTime? PaidAt { get; set; }
    public string? PaymentUrl { get; set; }
    public string? BookingStatus { get; set; }
}

public class ProcessPaymentRequestDto
{
    public Guid BookingId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? TransactionCode { get; set; }
}
