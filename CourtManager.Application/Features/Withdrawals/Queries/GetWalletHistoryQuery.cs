using MediatR;
using CourtManager.Application.DTOs;

namespace CourtManager.Application.Features.Withdrawals.Queries;

/// <summary>
/// Query to get owner's wallet history.
/// </summary>
public record GetWalletHistoryQuery(Guid OwnerId, int Page = 1, int PageSize = 20) : IRequest<WalletHistoryDto>;
