using MediatR;

namespace CourtManager.Application.Features.Withdrawals.Commands;

/// <summary>
/// Command to approve a withdrawal request.
/// </summary>
public class ApproveWithdrawalCommand : IRequest<bool>
{
    public Guid WithdrawalId { get; set; }
    public Guid AdminId { get; set; }
}
