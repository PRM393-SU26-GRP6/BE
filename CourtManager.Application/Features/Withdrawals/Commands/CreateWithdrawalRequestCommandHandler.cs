using MediatR;
using Microsoft.EntityFrameworkCore;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;

namespace CourtManager.Application.Features.Withdrawals.Commands;

/// <summary>
/// Handler for creating a withdrawal request.
/// </summary>
public class CreateWithdrawalRequestCommandHandler : IRequestHandler<CreateWithdrawalRequestCommand, WithdrawalRequestResultDto>
{
    private readonly IWithdrawalRepository _withdrawalRepository;
    private readonly IUserRepository _userRepository;

    public CreateWithdrawalRequestCommandHandler(
        IWithdrawalRepository withdrawalRepository,
        IUserRepository userRepository)
    {
        _withdrawalRepository = withdrawalRepository;
        _userRepository = userRepository;
    }

    public async Task<WithdrawalRequestResultDto> Handle(CreateWithdrawalRequestCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate amount
        if (request.Amount <= 0)
            throw new ValidationException("Amount must be greater than 0");

        // 2. Check user exists and get balance
        var user = await _userRepository.GetByIdWithWalletAsync(request.OwnerId, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), request.OwnerId);

        // 3. Check balance
        if (user.WalletBalance < request.Amount)
            throw new ValidationException($"Insufficient balance. Your balance is {user.WalletBalance:N0} VND");

        // 4. Deduct from wallet immediately (hold the money)
        await _userRepository.UpdateWalletBalanceAsync(request.OwnerId, -request.Amount, cancellationToken);

        // 5. Check no pending request exists
        var hasPending = await _withdrawalRepository.HasPendingRequestAsync(request.OwnerId, cancellationToken);
        if (hasPending)
        {
            // Refund the held amount since we're rejecting
            await _userRepository.UpdateWalletBalanceAsync(request.OwnerId, request.Amount, cancellationToken);
            throw new ValidationException("You already have a pending withdrawal request. Please wait for it to be processed.");
        }

        // 6. Create withdrawal request
        var withdrawal = new WithdrawalRequest
        {
            Id = Guid.NewGuid(),
            OwnerId = request.OwnerId,
            Amount = request.Amount,
            BankName = request.BankName,
            BankAccountNumber = request.BankAccountNumber,
            BankAccountHolderName = request.BankAccountHolderName,
            Status = WithdrawalStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _withdrawalRepository.AddAsync(withdrawal, cancellationToken);
        await _withdrawalRepository.SaveChangesAsync(cancellationToken);

        return new WithdrawalRequestResultDto
        {
            Id = withdrawal.Id,
            Status = withdrawal.Status.ToString(),
            Amount = withdrawal.Amount,
            CreatedAt = withdrawal.CreatedAt,
            Message = "Withdrawal request created successfully"
        };
    }
}
