using MediatR;

namespace CourtManager.Application.Features.Withdrawals.Commands;

/// <summary>
/// Command to reject a withdrawal request.
/// </summary>
public class RejectWithdrawalCommand : IRequest<bool>
{
    public Guid WithdrawalId { get; set; }
    public Guid AdminId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
