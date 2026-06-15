namespace CourtManager.Domain.Entities;

/// <summary>
/// Join table for Booking and Discount.
/// </summary>
public class BookingDiscount
{
    public Guid BookingId { get; set; }
    public Guid DiscountId { get; set; }
    public decimal DiscountAmount { get; set; }
    public bool IsDeleted { get; set; }

    // Navigation properties
    public Booking? Booking { get; set; }
    public Discount? Discount { get; set; }
}
