namespace CourtManager.Application.DTOs;

public class UpdateStatusDto
{
    public bool IsActive { get; set; }
}

public class UpdateSlotStatusDto
{
    public string SlotStatus { get; set; } = string.Empty;
}

public class VenueImageRequestDto
{
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class VenueAmenityRequestDto
{
    public Guid? AmenityId { get; set; }
    public List<Guid> AmenityIds { get; set; } = [];
}

public class BulkCreateSlotsDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string StartTime { get; set; } = "06:00";
    public string EndTime { get; set; } = "23:00";
    public int SlotDurationMinutes { get; set; } = 60;
    public decimal Price { get; set; }
}

public class StatusResultDto
{
    public Guid Id { get; set; }
    public string? Status { get; set; }
    public bool? IsActive { get; set; }
}

public class OwnerRevenueDto
{
    public string Key { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int Payments { get; set; }
}

public class BulkCreateSlotsResultDto
{
    public int CreatedSlots { get; set; }
}

