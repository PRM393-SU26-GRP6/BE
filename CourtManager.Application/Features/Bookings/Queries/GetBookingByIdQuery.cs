using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Queries;

/// <summary>
/// Query to retrieve a booking by ID.
/// </summary>
public class GetBookingByIdQuery : IRequest<BookingDto>
{
    public Guid BookingId { get; set; }
    public Guid UserId { get; set; }
    public bool IsOwner { get; set; }
    public bool IsAdmin { get; set; }

    public GetBookingByIdQuery(Guid bookingId, Guid userId = default, bool isOwner = false, bool isAdmin = false)
    {
        BookingId = bookingId;
        UserId = userId;
        IsOwner = isOwner;
        IsAdmin = isAdmin;
    }
}
