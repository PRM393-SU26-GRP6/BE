namespace CourtManager.Application.DTOs;

/// <summary>
/// Data Transfer Object for Booking.
/// </summary>
public class BookingDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid FieldId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public string BookingStatus { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<BookingItemDto> Items { get; set; } = [];
    public IEnumerable<PaymentDto> Payments { get; set; } = [];
}

public class BookingItemDto
{
    public Guid BookingItemId { get; set; }
    public Guid SlotId { get; set; }
    public Guid FieldId { get; set; }
    public string? FieldName { get; set; }
    public Guid VenueId { get; set; }
    public string? VenueName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal Price { get; set; }
}

/// <summary>
/// DTO for creating a new booking (request).
/// </summary>
public class CreateBookingDto
{
    public Guid UserId { get; set; }
    public Guid FieldId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
