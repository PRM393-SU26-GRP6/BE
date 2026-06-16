using MediatR;
using CourtManager.Application.DTOs;

namespace CourtManager.Application.Features.Withdrawals.Queries;

/// <summary>
/// Query to get admin wallet statistics.
/// </summary>
public record GetWalletStatsQuery : IRequest<WalletSummaryDto>;
