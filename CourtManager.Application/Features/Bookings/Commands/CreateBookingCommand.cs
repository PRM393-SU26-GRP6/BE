using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Commands;

/// <summary>
/// Command to create a new booking.
/// Implements CQRS Command pattern.
/// </summary>
public class CreateBookingCommand : IRequest<BookingDto>
{
    public Guid UserId { get; set; }
    public Guid FieldId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public Guid[] SlotIds { get; set; } = [];
    public string? DiscountCode { get; set; }
    public string? Note { get; set; }
}
