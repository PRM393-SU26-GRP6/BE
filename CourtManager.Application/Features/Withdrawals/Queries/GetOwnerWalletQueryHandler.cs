using MediatR;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;
using CourtManager.Domain.Enums;

namespace CourtManager.Application.Features.Withdrawals.Queries;

/// <summary>
/// Handler for getting owner's wallet summary.
/// </summary>
public class GetOwnerWalletQueryHandler : IRequestHandler<GetOwnerWalletQuery, OwnerWalletDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IWithdrawalRepository _withdrawalRepository;

    public GetOwnerWalletQueryHandler(
        IUserRepository userRepository,
        IWithdrawalRepository withdrawalRepository)
    {
        _userRepository = userRepository;
        _withdrawalRepository = withdrawalRepository;
    }

    public async Task<OwnerWalletDto> Handle(GetOwnerWalletQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdWithWalletAsync(request.OwnerId, cancellationToken);
        if (user == null)
        {
            return new OwnerWalletDto();
        }

        var pendingWithdrawals = await _withdrawalRepository.GetByOwnerIdAsync(request.OwnerId, cancellationToken);
        var pendingRequests = pendingWithdrawals.Where(w => w.Status == WithdrawalStatus.Pending).ToList();

        return new OwnerWalletDto
        {
            Balance = user.WalletBalance,
            PendingWithdrawalCount = pendingRequests.Count,
            PendingWithdrawalAmount = pendingRequests.Sum(w => w.Amount)
        };
    }
}
