using CourtManager.Domain.Enums;

namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a discount code.
/// </summary>
public class Discount
{
    public Guid DiscountId { get; set; }
    public Guid OwnerId { get; set; }
    public Guid? VenueId { get; set; } // Nullable: applies to all venues of owner if null
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DiscountType DiscountType { get; set; }
    public decimal Value { get; set; }
    public decimal MinBookingAmount { get; set; }
    public decimal MaxDiscountAmount { get; set; }
    public int UsageLimit { get; set; }
    public int UsedCount { get; set; } = 0;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public User? Owner { get; set; }
    public Venue? Venue { get; set; }
    public ICollection<BookingDiscount> BookingDiscounts { get; set; } = [];
}
