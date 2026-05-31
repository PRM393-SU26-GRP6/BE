namespace CourtManager.Application.DTOs;

public class PaymentGatewayCallbackDto
{
    public string TransactionCode { get; set; } = string.Empty;
    public bool Success { get; set; }
}

public class PaymentGatewayCallbackResultDto
{
    public int StatusCode { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? PaymentId { get; set; }
    public string? PaymentStatus { get; set; }
}

public class SePayQrResponseDto
{
    public string QrUrl { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid PaymentId { get; set; }
    public string Status { get; set; } = string.Empty;
    public BankInfoDto BankInfo { get; set; } = new();
}

public class BankInfoDto
{
    public string BankId { get; set; } = string.Empty;
    public string AccountNo { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
}
