using MediatR;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;
using CourtManager.Domain.Enums;

namespace CourtManager.Application.Features.Withdrawals.Queries;

/// <summary>
/// Handler for getting all withdrawal requests for admin.
/// </summary>
public class GetAdminWithdrawalRequestsQueryHandler : IRequestHandler<GetAdminWithdrawalRequestsQuery, IEnumerable<WithdrawalRequestDto>>
{
    private readonly IWithdrawalRepository _withdrawalRepository;

    public GetAdminWithdrawalRequestsQueryHandler(IWithdrawalRepository withdrawalRepository)
    {
        _withdrawalRepository = withdrawalRepository;
    }

    public async Task<IEnumerable<WithdrawalRequestDto>> Handle(GetAdminWithdrawalRequestsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.WithdrawalRequest> withdrawals;

        if (request.Status.HasValue)
        {
            withdrawals = await _withdrawalRepository.GetByStatusAsync(request.Status.Value, cancellationToken);
        }
        else
        {
            withdrawals = await _withdrawalRepository.GetAllAsync(cancellationToken);
        }

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
            ApprovedByAdminId = w.ApprovedByAdminId,
            ApprovedByAdminName = w.ApprovedByAdmin?.FullName,
            RejectionReason = w.RejectionReason,
            CreatedAt = w.CreatedAt,
            ApprovedAt = w.ApprovedAt
        });
    }
}
