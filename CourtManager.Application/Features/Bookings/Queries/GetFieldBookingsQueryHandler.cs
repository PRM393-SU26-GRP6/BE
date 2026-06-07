using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Queries;

/// <summary>
/// Handler for GetFieldBookingsQuery.
/// Retrieves all bookings for a specific football field.
/// </summary>
public class GetFieldBookingsQueryHandler : IRequestHandler<GetFieldBookingsQuery, IEnumerable<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;

    public GetFieldBookingsQueryHandler(IBookingRepository bookingRepository, IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BookingDto>> Handle(GetFieldBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _bookingRepository.GetBookingsByCourtIdAsync(request.FieldId, cancellationToken);
        return _mapper.Map<IEnumerable<BookingDto>>(bookings);
    }
}
