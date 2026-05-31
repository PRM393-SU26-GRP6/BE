using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Domain.Enums;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Queries;

/// <summary>
/// Handler for GetUserBookingsQuery.
/// Retrieves all bookings for a specific user.
/// </summary>
public class GetUserBookingsQueryHandler : IRequestHandler<GetUserBookingsQuery, IEnumerable<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;

    public GetUserBookingsQueryHandler(IBookingRepository bookingRepository, IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BookingDto>> Handle(GetUserBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _bookingRepository.GetBookingsByUserIdAsync(request.UserId, cancellationToken);
        var query = bookings.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<BookingStatus>(request.Status, true, out var status))
            {
                query = query.Where(b => b.BookingStatus == status);
            }
            else
            {
                query = [];
            }
        }

        if (request.From.HasValue)
        {
            query = query.Where(b => b.CreatedAt >= request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.Where(b => b.CreatedAt <= request.To.Value);
        }

        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        query = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return _mapper.Map<IEnumerable<BookingDto>>(query);
    }
}
