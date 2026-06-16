using MediatR;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;

namespace CourtManager.Application.Features.Withdrawals.Queries;

/// <summary>
/// Handler for getting owner's wallet history with pagination.
/// </summary>
public class GetWalletHistoryQueryHandler : IRequestHandler<GetWalletHistoryQuery, WalletHistoryDto>
{
    private readonly IWalletTransactionRepository _walletTransactionRepository;
    private readonly IUserRepository _userRepository;

    public GetWalletHistoryQueryHandler(
        IWalletTransactionRepository walletTransactionRepository,
        IUserRepository userRepository)
    {
        _walletTransactionRepository = walletTransactionRepository;
        _userRepository = userRepository;
    }

    public async Task<WalletHistoryDto> Handle(GetWalletHistoryQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdWithWalletAsync(request.OwnerId, cancellationToken);
        if (user == null)
        {
            return new WalletHistoryDto();
        }

        var transactions = await _walletTransactionRepository.GetByOwnerIdPagedAsync(
            request.OwnerId, request.Page, request.PageSize, cancellationToken);

        var totalCount = await _walletTransactionRepository.GetCountByOwnerIdAsync(request.OwnerId, cancellationToken);

        return new WalletHistoryDto
        {
            Transactions = transactions.Select(t => new WalletTransactionDto
            {
                Id = t.Id,
                OwnerId = t.OwnerId,
                Type = t.Type.ToString(),
                Amount = t.Amount,
                Description = t.Description,
                RelatedBookingId = t.RelatedBookingId,
                RelatedWithdrawalId = t.RelatedWithdrawalId,
                CreatedAt = t.CreatedAt
            }).ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}
