using CourtManager.Application.DTOs;
using CourtManager.Domain.Enums;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Owner;

public record GetOwnerStatsQuery(Guid OwnerId) : IRequest<OwnerStatsDto>;

public class GetOwnerStatsQueryHandler : IRequestHandler<GetOwnerStatsQuery, OwnerStatsDto>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IBookingRepository _bookingRepository;

    public GetOwnerStatsQueryHandler(IVenueRepository venueRepository, IBookingRepository bookingRepository)
    {
        _venueRepository = venueRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<OwnerStatsDto> Handle(GetOwnerStatsQuery request, CancellationToken cancellationToken)
    {
        var venues = (await _venueRepository.GetOwnerVenuesAsync(request.OwnerId, null, 0, int.MaxValue, cancellationToken)).ToList();
        var bookings = (await _bookingRepository.GetBookingsForOwnerAsync(request.OwnerId, cancellationToken)).ToList();
        var successfulPayments = bookings
            .SelectMany(b => b.Payments)
            .Where(p => p.PaymentStatus == PaymentStatus.Success)
            .ToList();

        return new OwnerStatsDto
        {
            TotalVenues = venues.Count,
            TotalFields = venues.Sum(v => v.FootballFields.Count(f => f.IsActive)),
            PendingBookings = bookings.Count(b => b.BookingStatus == BookingStatus.Pending),
            AcceptedBookings = bookings.Count(b => b.BookingStatus == BookingStatus.Accepted || b.BookingStatus == BookingStatus.Deposited),
            CompletedBookings = bookings.Count(b => b.BookingStatus == BookingStatus.Completed),
            TotalRevenue = successfulPayments.Sum(p => p.Amount),
            DepositRevenue = successfulPayments.Where(p => p.PaymentType == PaymentType.Deposit).Sum(p => p.Amount),
            FinalPaymentRevenue = successfulPayments.Where(p => p.PaymentType == PaymentType.Final).Sum(p => p.Amount)
        };
    }
}
