using CourtManager.Application.Exceptions;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Venues.Commands;

public class UpdateVenueStatusCommandHandler : IRequestHandler<UpdateVenueStatusCommand, UpdateVenueStatusResult>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IBookingRepository _bookingRepository;

    public UpdateVenueStatusCommandHandler(
        IVenueRepository venueRepository,
        IBookingRepository bookingRepository)
    {
        _venueRepository = venueRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<UpdateVenueStatusResult> Handle(UpdateVenueStatusCommand request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);
        if (venue == null || venue.IsDeleted)
        {
            throw new NotFoundException("Venue", request.VenueId);
        }

        if (venue.OwnerId != request.OwnerId)
        {
            throw new ForbiddenException("You are not the owner of this venue.");
        }

        // Block deactivation when there are still in-flight bookings
        if (!request.IsActive)
        {
            var hasActive = await _bookingRepository.HasActiveBookingsForVenueAsync(request.VenueId, cancellationToken);
            if (hasActive)
            {
                throw new InvalidOperationException(
                    "Cannot deactivate this venue: there are still active bookings (pending/accepted/deposited). " +
                    "Please resolve all active bookings first.");
            }
        }

        venue.IsActive = request.IsActive;
        venue.UpdatedAt = DateTime.UtcNow;

        await _venueRepository.UpdateAsync(venue, cancellationToken);
        await _venueRepository.SaveChangesAsync(cancellationToken);

        var action = request.IsActive ? "activated" : "deactivated";
        return new UpdateVenueStatusResult(venue.VenueId, venue.IsActive, $"Venue {action} successfully.");
    }
}
