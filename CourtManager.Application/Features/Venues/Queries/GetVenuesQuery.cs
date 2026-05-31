using CourtManager.Application.DTOs;
using CourtManager.Domain.Enums;
using MediatR;

namespace CourtManager.Application.Features.Venues.Queries;

public record GetVenuesQuery : IRequest<PagedResult<VenueDto>>
{
    public string? Q { get; init; }
    public FieldType? FieldType { get; init; }
    public string? AmenityIds { get; init; } // Changed to string to be parsed in Handler
    public double? MinRating { get; init; }
    public decimal? PriceMin { get; init; }
    public decimal? PriceMax { get; init; }
    public string? Sort { get; init; }
    
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
