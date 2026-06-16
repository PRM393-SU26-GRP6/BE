using CourtManager.Domain.Enums;

namespace CourtManager.Application.DTOs;

/// <summary>
/// Data Transfer Object for WithdrawalRequest.
/// </summary>
public class WithdrawalRequestDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public decimal Amount { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string BankAccountNumber { get; set; } = string.Empty;
    public string BankAccountHolderName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? ApprovedByAdminId { get; set; }
    public string? ApprovedByAdminName { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
}

/// <summary>
/// Request to create a withdrawal request.
/// </summary>
public class CreateWithdrawalRequestDto
{
    public decimal Amount { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string BankAccountNumber { get; set; } = string.Empty;
    public string BankAccountHolderName { get; set; } = string.Empty;
}

/// <summary>
/// Request to reject a withdrawal request.
/// </summary>
public class RejectWithdrawalRequestDto
{
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Response for withdrawal request result.
/// </summary>
public class WithdrawalRequestResultDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Message { get; set; } = string.Empty;
}
