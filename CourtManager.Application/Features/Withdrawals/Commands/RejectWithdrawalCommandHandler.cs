using MediatR;
using CourtManager.Application.Interfaces;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Enums;

namespace CourtManager.Application.Features.Withdrawals.Commands;

/// <summary>
/// Handler for rejecting a withdrawal request.
/// </summary>
public class RejectWithdrawalCommandHandler : IRequestHandler<RejectWithdrawalCommand, bool>
{
    private readonly IWithdrawalRepository _withdrawalRepository;
    private readonly IUserRepository _userRepository;
    private readonly IWalletTransactionRepository _walletTransactionRepository;

    public RejectWithdrawalCommandHandler(
        IWithdrawalRepository withdrawalRepository,
        IUserRepository userRepository,
        IWalletTransactionRepository walletTransactionRepository)
    {
        _withdrawalRepository = withdrawalRepository;
        _userRepository = userRepository;
        _walletTransactionRepository = walletTransactionRepository;
    }

    public async Task<bool> Handle(RejectWithdrawalCommand request, CancellationToken cancellationToken)
    {
        var withdrawal = await _withdrawalRepository.GetByIdAsync(request.WithdrawalId, cancellationToken);
        if (withdrawal == null)
            throw new NotFoundException(nameof(Domain.Entities.WithdrawalRequest), request.WithdrawalId);

        if (withdrawal.Status != WithdrawalStatus.Pending)
            throw new ValidationException($"Cannot reject withdrawal. Current status is '{withdrawal.Status}'");

        // Refund the held amount back to user's wallet
        await _userRepository.UpdateWalletBalanceAsync(withdrawal.OwnerId, withdrawal.Amount, cancellationToken);

        // Create wallet transaction record for refund
        var walletTransaction = new Domain.Entities.WalletTransaction
        {
            Id = Guid.NewGuid(),
            OwnerId = withdrawal.OwnerId,
            Type = WalletTransactionType.Refund,
            Amount = withdrawal.Amount,
            Description = $"Withdrawal rejected: {request.Reason}",
            RelatedWithdrawalId = withdrawal.Id,
            CreatedAt = DateTime.UtcNow
        };
        await _walletTransactionRepository.AddAsync(walletTransaction, cancellationToken);

        withdrawal.Status = WithdrawalStatus.Rejected;
        withdrawal.RejectionReason = request.Reason;
        withdrawal.ApprovedByAdminId = request.AdminId;

        await _withdrawalRepository.UpdateAsync(withdrawal, cancellationToken);
        await _withdrawalRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
