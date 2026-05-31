using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Queries;

public record GetOwnerPendingBookingsQuery(Guid OwnerId) : IRequest<IEnumerable<BookingDto>>;
public record GetOwnerBookingsQuery(Guid OwnerId) : IRequest<IEnumerable<BookingDto>>;

public class GetOwnerPendingBookingsQueryHandler : IRequestHandler<GetOwnerPendingBookingsQuery, IEnumerable<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;

    public GetOwnerPendingBookingsQueryHandler(IBookingRepository bookingRepository, IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BookingDto>> Handle(GetOwnerPendingBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _bookingRepository.GetPendingBookingsForOwnerAsync(request.OwnerId, cancellationToken);
        return _mapper.Map<IEnumerable<BookingDto>>(bookings);
    }
}

public class GetOwnerBookingsQueryHandler : IRequestHandler<GetOwnerBookingsQuery, IEnumerable<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;

    public GetOwnerBookingsQueryHandler(IBookingRepository bookingRepository, IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BookingDto>> Handle(GetOwnerBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _bookingRepository.GetBookingsForOwnerAsync(request.OwnerId, cancellationToken);
        return _mapper.Map<IEnumerable<BookingDto>>(bookings);
    }
}
