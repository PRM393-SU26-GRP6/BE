using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Queries;

/// <summary>
/// Query to retrieve all bookings for a specific user.
/// </summary>
public class GetUserBookingsQuery : IRequest<IEnumerable<BookingDto>>
{
    /// <summary>
    /// The user ID to fetch bookings for.
    /// </summary>
    public Guid UserId { get; set; }
    public string? Status { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public GetUserBookingsQuery(Guid userId, string? status = null, DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 20)
    {
        UserId = userId;
        Status = status;
        From = from;
        To = to;
        Page = page;
        PageSize = pageSize;
    }
}
