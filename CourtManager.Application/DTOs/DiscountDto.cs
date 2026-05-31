namespace CourtManager.Application.DTOs;

public class DiscountDto
{
    public Guid DiscountId { get; set; }
    public Guid OwnerId { get; set; }
    public Guid? FieldId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal MinBookingAmount { get; set; }
    public decimal MaxDiscountAmount { get; set; }
    public int UsageLimit { get; set; }
    public int UsedCount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}

public class ValidateDiscountRequestDto
{
    public string Code { get; set; } = string.Empty;
    public Guid? FieldId { get; set; }
    public Guid[] SlotIds { get; set; } = [];
    public decimal TotalAmount { get; set; }
}

public class ValidateDiscountResponseDto
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? DiscountId { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
}
