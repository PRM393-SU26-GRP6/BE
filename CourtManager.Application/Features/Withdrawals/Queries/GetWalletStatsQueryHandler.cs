using MediatR;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;
using CourtManager.Domain.Enums;

namespace CourtManager.Application.Features.Withdrawals.Queries;

/// <summary>
/// Handler for getting admin wallet statistics.
/// </summary>
public class GetWalletStatsQueryHandler : IRequestHandler<GetWalletStatsQuery, WalletSummaryDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IWithdrawalRepository _withdrawalRepository;
    private readonly IWalletTransactionRepository _walletTransactionRepository;

    public GetWalletStatsQueryHandler(
        IUserRepository userRepository,
        IWithdrawalRepository withdrawalRepository,
        IWalletTransactionRepository walletTransactionRepository)
    {
        _userRepository = userRepository;
        _withdrawalRepository = withdrawalRepository;
        _walletTransactionRepository = walletTransactionRepository;
    }

    public async Task<WalletSummaryDto> Handle(GetWalletStatsQuery request, CancellationToken cancellationToken)
    {
        // Get total balance of all owners
        var owners = await _userRepository.GetOwnersWithWalletsAsync(cancellationToken);
        var totalBalance = owners.Sum(o => o.WalletBalance);

        // Get pending withdrawal amount
        var pendingWithdrawals = await _withdrawalRepository.GetAllPendingAsync(cancellationToken);
        var pendingAmount = pendingWithdrawals.Sum(w => w.Amount);

        // Get total withdrawn (approved withdrawals)
        var approvedWithdrawals = await _withdrawalRepository.GetByStatusAsync(WithdrawalStatus.Approved, cancellationToken);
        var totalWithdrawn = approvedWithdrawals.Sum(w => w.Amount);

        // Get total earnings and transaction count
        var allTransactions = await _walletTransactionRepository.GetAllAsync(cancellationToken);
        var totalEarnings = allTransactions
            .Where(t => t.Type == WalletTransactionType.Deposit || t.Type == WalletTransactionType.Refund)
            .Sum(t => t.Amount);
        var transactionCount = allTransactions.Count();

        return new WalletSummaryDto
        {
            Balance = totalBalance,
            PendingWithdrawal = pendingAmount,
            TotalWithdrawn = totalWithdrawn,
            TotalEarnings = totalEarnings,
            TransactionCount = transactionCount
        };
    }
}
