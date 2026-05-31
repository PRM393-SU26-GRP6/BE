using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Queries;

/// <summary>
/// Handler for GetBookingByIdQuery.
/// Retrieves a booking by its ID.
/// </summary>
public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, BookingDto>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;

    public GetBookingByIdQueryHandler(IBookingRepository bookingRepository, IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _mapper = mapper;
    }

    public async Task<BookingDto> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
            throw new NotFoundException(nameof(Booking), request.BookingId);

        var isBookingUser = booking.UserId == request.UserId;
        var isBookingOwner = request.IsOwner && booking.BookingItems.Any(i =>
            i.Slot?.Field?.Venue?.OwnerId == request.UserId);

        if (!request.IsAdmin && !isBookingUser && !isBookingOwner)
            throw new ValidationException("You are not allowed to view this booking.");

        return _mapper.Map<BookingDto>(booking);
    }
}
