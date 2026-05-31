using MediatR;

namespace CourtManager.Application.Features.Venues.Commands;

/// <summary>
/// Toggles a venue's active status (on/off).
/// Owner cannot deactivate a venue that still has active bookings.
/// </summary>
public record UpdateVenueStatusCommand(
    Guid VenueId,
    Guid OwnerId,
    bool IsActive
) : IRequest<UpdateVenueStatusResult>;

public record UpdateVenueStatusResult(
    Guid VenueId,
    bool IsActive,
    string Message
);
