using MediatR;
using CourtManager.Application.DTOs;

namespace CourtManager.Application.Features.Withdrawals.Commands;

/// <summary>
/// Command to create a new withdrawal request.
/// </summary>
public class CreateWithdrawalRequestCommand : IRequest<WithdrawalRequestResultDto>
{
    public Guid OwnerId { get; set; }
    public decimal Amount { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string BankAccountNumber { get; set; } = string.Empty;
    public string BankAccountHolderName { get; set; } = string.Empty;
}
