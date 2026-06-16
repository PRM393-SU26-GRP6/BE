using MediatR;
using CourtManager.Application.DTOs;

namespace CourtManager.Application.Features.Withdrawals.Queries;

/// <summary>
/// Query to get owner's wallet summary.
/// </summary>
public record GetOwnerWalletQuery(Guid OwnerId) : IRequest<OwnerWalletDto>;
