using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Queries;

/// <summary>
/// Handler for SearchBookingsQuery.
/// Searches and filters bookings with advanced criteria.
/// </summary>
public class SearchBookingsQueryHandler : IRequestHandler<SearchBookingsQuery, IEnumerable<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;

    public SearchBookingsQueryHandler(IBookingRepository bookingRepository, IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BookingDto>> Handle(SearchBookingsQuery request, CancellationToken cancellationToken)
    {
        // Get all bookings (in production, consider pagination at repository level)
        var allBookings = await _bookingRepository.GetAllAsync(cancellationToken);

        // Apply filters
        var filtered = allBookings
            .Where(b =>
                (!request.UserId.HasValue || b.UserId == request.UserId))
                /*
                &&
                (!request.FieldId.HasValue || b.FieldId == request.FieldId) &&
                (string.IsNullOrEmpty(request.BookingStatus) || b.BookingStatus == request.BookingStatus) &&
                (!request.StartDate.HasValue || b.StartTime.Date >= request.StartDate.Value.Date) &&
                (!request.EndDate.HasValue || b.EndTime.Date <= request.EndDate.Value.Date))
                */
            .OrderByDescending(b => b.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return _mapper.Map<IEnumerable<BookingDto>>(filtered);
    }
}
