using MediatR;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;

namespace CourtManager.Application.Features.Withdrawals.Queries;

/// <summary>
/// Query to get withdrawal requests for an owner.
/// </summary>
public record GetOwnerWithdrawalRequestsQuery(Guid OwnerId) : IRequest<IEnumerable<WithdrawalRequestDto>>;
