using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Venues.Queries;

/// <summary>
/// Gets the list of venues owned by the currently logged-in owner.
/// </summary>
public record GetOwnerVenuesQuery : IRequest<PagedResult<VenueDto>>
{
    public Guid OwnerId { get; init; }
    public bool? IsActive { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
