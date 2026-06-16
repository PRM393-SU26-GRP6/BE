using MediatR;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;
using CourtManager.Application.Mappings;
using CourtManager.Domain.Enums;

namespace CourtManager.Application.Features.Withdrawals.Queries;

/// <summary>
/// Handler for getting owner's withdrawal requests.
/// </summary>
public class GetOwnerWithdrawalRequestsQueryHandler : IRequestHandler<GetOwnerWithdrawalRequestsQuery, IEnumerable<WithdrawalRequestDto>>
{
    private readonly IWithdrawalRepository _withdrawalRepository;

    public GetOwnerWithdrawalRequestsQueryHandler(IWithdrawalRepository withdrawalRepository)
    {
        _withdrawalRepository = withdrawalRepository;
    }

    public async Task<IEnumerable<WithdrawalRequestDto>> Handle(GetOwnerWithdrawalRequestsQuery request, CancellationToken cancellationToken)
    {
        var withdrawals = await _withdrawalRepository.GetByOwnerIdAsync(request.OwnerId, cancellationToken);
        return withdrawals.Select(w => new WithdrawalRequestDto
        {
            Id = w.Id,
            OwnerId = w.OwnerId,
            OwnerName = w.Owner?.FullName,
            Amount = w.Amount,
            BankName = w.BankName,
            BankAccountNumber = w.BankAccountNumber,
            BankAccountHolderName = w.BankAccountHolderName,
            Status = w.Status.ToString(),
            RejectionReason = w.RejectionReason,
            CreatedAt = w.CreatedAt,
            ApprovedAt = w.ApprovedAt
        });
    }
}
