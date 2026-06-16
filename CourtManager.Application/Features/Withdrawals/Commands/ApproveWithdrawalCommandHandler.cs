using MediatR;
using CourtManager.Application.Interfaces;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;

namespace CourtManager.Application.Features.Withdrawals.Commands;

/// <summary>
/// Handler for approving a withdrawal request.
/// Uses atomic update for concurrency safety.
/// </summary>
public class ApproveWithdrawalCommandHandler : IRequestHandler<ApproveWithdrawalCommand, bool>
{
    private readonly IWithdrawalRepository _withdrawalRepository;
    private readonly IUserRepository _userRepository;
    private readonly IWalletTransactionRepository _walletTransactionRepository;

    public ApproveWithdrawalCommandHandler(
        IWithdrawalRepository withdrawalRepository,
        IUserRepository userRepository,
        IWalletTransactionRepository walletTransactionRepository)
    {
        _withdrawalRepository = withdrawalRepository;
        _userRepository = userRepository;
        _walletTransactionRepository = walletTransactionRepository;
    }

    public async Task<bool> Handle(ApproveWithdrawalCommand request, CancellationToken cancellationToken)
    {
        var withdrawal = await _withdrawalRepository.GetByIdAsync(request.WithdrawalId, cancellationToken);
        if (withdrawal == null)
            throw new NotFoundException(nameof(WithdrawalRequest), request.WithdrawalId);

        if (withdrawal.Status != WithdrawalStatus.Pending)
            throw new ValidationException($"Cannot approve withdrawal. Current status is '{withdrawal.Status}'");

        // Get user with current balance for validation
        var user = await _userRepository.GetByIdWithWalletAsync(withdrawal.OwnerId, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), withdrawal.OwnerId);

        if (user.WalletBalance < withdrawal.Amount)
            throw new ValidationException($"Insufficient balance. User has {user.WalletBalance:N0} VND but needs {withdrawal.Amount:N0} VND");

        // Deduct balance atomically
        await _userRepository.UpdateWalletBalanceAsync(withdrawal.OwnerId, -withdrawal.Amount, cancellationToken);

        // Create wallet transaction record
        var walletTransaction = new WalletTransaction
        {
            Id = Guid.NewGuid(),
            OwnerId = withdrawal.OwnerId,
            Type = WalletTransactionType.Withdrawal,
            Amount = withdrawal.Amount,
            Description = $"Withdrawal to {withdrawal.BankName} account ({withdrawal.BankAccountNumber})",
            RelatedWithdrawalId = withdrawal.Id,
            CreatedAt = DateTime.UtcNow
        };
        await _walletTransactionRepository.AddAsync(walletTransaction, cancellationToken);

        // Update withdrawal status
        withdrawal.Status = WithdrawalStatus.Approved;
        withdrawal.ApprovedByAdminId = request.AdminId;
        withdrawal.ApprovedAt = DateTime.UtcNow;
        await _withdrawalRepository.UpdateAsync(withdrawal, cancellationToken);
        await _withdrawalRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
