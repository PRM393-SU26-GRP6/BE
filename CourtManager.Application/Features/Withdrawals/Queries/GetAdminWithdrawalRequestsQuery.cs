using MediatR;
using CourtManager.Application.DTOs;
using CourtManager.Domain.Enums;

namespace CourtManager.Application.Features.Withdrawals.Queries;

/// <summary>
/// Query to get all withdrawal requests for admin.
/// </summary>
public record GetAdminWithdrawalRequestsQuery(WithdrawalStatus? Status = null) : IRequest<IEnumerable<WithdrawalRequestDto>>;
