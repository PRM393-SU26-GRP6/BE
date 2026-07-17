using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Application.Features.TimeSlots;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;
using CourtManager.Application.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Owner;

public record GetOwnerRevenueQuery(Guid OwnerId, DateTime? From, DateTime? To, string GroupBy) : IRequest<IEnumerable<OwnerRevenueDto>>;
public record UpdateOwnerVenueStatusCommand(Guid OwnerId, Guid VenueId, UpdateStatusDto Request) : IRequest<StatusResultDto>;
public record AddVenueImageCommand(Guid OwnerId, Guid VenueId, VenueImageRequestDto Request) : IRequest<VenueImage>;
public record DeleteVenueImageCommand(Guid OwnerId, Guid VenueId, Guid ImageId) : IRequest<bool>;
public record AddVenueAmenitiesCommand(Guid OwnerId, Guid VenueId, VenueAmenityRequestDto Request) : IRequest<IEnumerable<string>>;
public record DeleteVenueAmenityCommand(Guid OwnerId, Guid VenueId, Guid AmenityId) : IRequest<bool>;
public record UpdateFieldStatusCommand(Guid OwnerId, Guid FieldId, UpdateStatusDto Request) : IRequest<StatusResultDto>;
public record BulkCreateSlotsCommand(Guid OwnerId, Guid FieldId, BulkCreateSlotsDto Request) : IRequest<BulkCreateSlotsResultDto>;
public record UpdateSlotStatusCommand(Guid OwnerId, Guid SlotId, UpdateSlotStatusDto Request) : IRequest<StatusResultDto>;
public record CompleteBookingCommand(Guid OwnerId, Guid BookingId) : IRequest<StatusResultDto>;
public record UpdateDiscountStatusCommand(Guid OwnerId, Guid DiscountId, UpdateStatusDto Request) : IRequest<StatusResultDto>;

public class GetOwnerRevenueQueryHandler : IRequestHandler<GetOwnerRevenueQuery, IEnumerable<OwnerRevenueDto>>
{
    private readonly IBookingRepository _bookingRepository;

    public GetOwnerRevenueQueryHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<IEnumerable<OwnerRevenueDto>> Handle(GetOwnerRevenueQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _bookingRepository.GetBookingsForOwnerAsync(request.OwnerId, cancellationToken);
        var payments = bookings
            .SelectMany(b => b.Payments.Select(p => new
            {
                Payment = p,
                VenueId = b.BookingItems.Select(i => i.Slot?.Field?.VenueId).FirstOrDefault(),
                VenueName = b.BookingItems.Select(i => i.Slot?.Field?.Venue?.VenueName).FirstOrDefault()
            }))
            .Where(x => x.Payment.PaymentStatus == PaymentStatus.Success && x.Payment.PaidAt.HasValue)
            .Where(x => !request.From.HasValue || x.Payment.PaidAt!.Value.Date >= request.From.Value.Date)
            .Where(x => !request.To.HasValue || x.Payment.PaidAt!.Value.Date <= request.To.Value.Date)
            .ToList();

        return request.GroupBy.Trim().ToLowerInvariant() switch
        {
            "month" => payments
                .GroupBy(x => x.Payment.PaidAt!.Value.ToString("yyyy-MM"))
                .Select(g => new OwnerRevenueDto { Key = g.Key, Revenue = g.Sum(x => x.Payment.Amount), Payments = g.Count() })
                .ToList(),
            "venue" => payments
                .GroupBy(x => x.VenueName ?? x.VenueId?.ToString() ?? "unknown")
                .Select(g => new OwnerRevenueDto { Key = g.Key, Revenue = g.Sum(x => x.Payment.Amount), Payments = g.Count() })
                .ToList(),
            _ => payments
                .GroupBy(x => x.Payment.PaidAt!.Value.Date.ToString("yyyy-MM-dd"))
                .Select(g => new OwnerRevenueDto { Key = g.Key, Revenue = g.Sum(x => x.Payment.Amount), Payments = g.Count() })
                .ToList()
        };
    }
}

public class UpdateOwnerVenueStatusCommandHandler : IRequestHandler<UpdateOwnerVenueStatusCommand, StatusResultDto>
{
    private readonly IVenueRepository _venueRepository;

    public UpdateOwnerVenueStatusCommandHandler(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<StatusResultDto> Handle(UpdateOwnerVenueStatusCommand request, CancellationToken cancellationToken)
    {
        var venue = await GetOwnedVenue(_venueRepository, request.VenueId, request.OwnerId, cancellationToken);
        venue.IsActive = request.Request.IsActive;
        venue.UpdatedAt = DateTime.UtcNow;
        await _venueRepository.UpdateAsync(venue, cancellationToken);
        await _venueRepository.SaveChangesAsync(cancellationToken);
        return new StatusResultDto { Id = request.VenueId, IsActive = venue.IsActive };
    }

    internal static async Task<Venue> GetOwnedVenue(IVenueRepository venueRepository, Guid venueId, Guid ownerId, CancellationToken cancellationToken)
    {
        var venue = await venueRepository.GetVenueByIdAsync(venueId, cancellationToken);
        if (venue == null)
        {
            throw new NotFoundException(nameof(Venue), venueId);
        }

        if (venue.OwnerId != ownerId)
        {
            throw new ValidationException("Only the venue owner can manage this venue.");
        }

        return venue;
    }
}

public class AddVenueImageCommandHandler : IRequestHandler<AddVenueImageCommand, VenueImage>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IVenueImageRepository _venueImageRepository;

    public AddVenueImageCommandHandler(IVenueRepository venueRepository, IVenueImageRepository venueImageRepository)
    {
        _venueRepository = venueRepository;
        _venueImageRepository = venueImageRepository;
    }

    public async Task<VenueImage> Handle(AddVenueImageCommand request, CancellationToken cancellationToken)
    {
        await UpdateOwnerVenueStatusCommandHandler.GetOwnedVenue(_venueRepository, request.VenueId, request.OwnerId, cancellationToken);

        var image = new VenueImage
        {
            ImageId = Guid.NewGuid(),
            VenueId = request.VenueId,
            ImageUrl = request.Request.ImageUrl,
            IsPrimary = request.Request.IsPrimary
        };

        await _venueImageRepository.AddAsync(image, cancellationToken);
        await _venueImageRepository.SaveChangesAsync(cancellationToken);
        return image;
    }
}

public class DeleteVenueImageCommandHandler : IRequestHandler<DeleteVenueImageCommand, bool>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IVenueImageRepository _venueImageRepository;

    public DeleteVenueImageCommandHandler(IVenueRepository venueRepository, IVenueImageRepository venueImageRepository)
    {
        _venueRepository = venueRepository;
        _venueImageRepository = venueImageRepository;
    }

    public async Task<bool> Handle(DeleteVenueImageCommand request, CancellationToken cancellationToken)
    {
        await UpdateOwnerVenueStatusCommandHandler.GetOwnedVenue(_venueRepository, request.VenueId, request.OwnerId, cancellationToken);

        var image = await _venueImageRepository.GetByIdAsync(request.ImageId, cancellationToken);
        if (image == null || image.VenueId != request.VenueId)
        {
            throw new NotFoundException(nameof(VenueImage), request.ImageId);
        }

        image.IsDeleted = true;
        image.DeletedAt = DateTime.UtcNow;
        await _venueImageRepository.UpdateAsync(image, cancellationToken);
        await _venueImageRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public class AddVenueAmenitiesCommandHandler : IRequestHandler<AddVenueAmenitiesCommand, IEnumerable<string>>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IAmenityRepository _amenityRepository;
    private readonly IVenueAmenityRepository _venueAmenityRepository;

    public AddVenueAmenitiesCommandHandler(
        IVenueRepository venueRepository,
        IAmenityRepository amenityRepository,
        IVenueAmenityRepository venueAmenityRepository)
    {
        _venueRepository = venueRepository;
        _amenityRepository = amenityRepository;
        _venueAmenityRepository = venueAmenityRepository;
    }

    public async Task<IEnumerable<string>> Handle(AddVenueAmenitiesCommand request, CancellationToken cancellationToken)
    {
        await UpdateOwnerVenueStatusCommandHandler.GetOwnedVenue(_venueRepository, request.VenueId, request.OwnerId, cancellationToken);

        var amenityIds = (request.Request.AmenityIds != null && request.Request.AmenityIds.Count > 0)
            ? request.Request.AmenityIds
            : request.Request.AmenityId.HasValue
                ? [request.Request.AmenityId.Value]
                : [];

        foreach (var amenityId in amenityIds.Distinct())
        {
            var amenity = await _amenityRepository.GetByIdAsync(amenityId, cancellationToken);
            if (amenity == null)
            {
                throw new NotFoundException(nameof(Amenity), amenityId);
            }

            if (!await _venueAmenityRepository.ExistsAsync(request.VenueId, amenityId, cancellationToken))
            {
                await _venueAmenityRepository.AddAsync(new VenueAmenity { VenueId = request.VenueId, AmenityId = amenityId }, cancellationToken);
            }
        }

        await _venueAmenityRepository.SaveChangesAsync(cancellationToken);
        var updated = await _venueRepository.GetVenueByIdAsync(request.VenueId, cancellationToken);
        return updated?.VenueAmenities.Select(va => va.Amenity?.Name).Where(name => name != null).Cast<string>() ?? [];
    }
}

public class DeleteVenueAmenityCommandHandler : IRequestHandler<DeleteVenueAmenityCommand, bool>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IVenueAmenityRepository _venueAmenityRepository;

    public DeleteVenueAmenityCommandHandler(IVenueRepository venueRepository, IVenueAmenityRepository venueAmenityRepository)
    {
        _venueRepository = venueRepository;
        _venueAmenityRepository = venueAmenityRepository;
    }

    public async Task<bool> Handle(DeleteVenueAmenityCommand request, CancellationToken cancellationToken)
    {
        await UpdateOwnerVenueStatusCommandHandler.GetOwnedVenue(_venueRepository, request.VenueId, request.OwnerId, cancellationToken);

        var venueAmenity = await _venueAmenityRepository.GetAsync(request.VenueId, request.AmenityId, cancellationToken);
        if (venueAmenity == null)
        {
            throw new NotFoundException(nameof(VenueAmenity), request.AmenityId);
        }

        await _venueAmenityRepository.DeleteAsync(venueAmenity, cancellationToken);
        await _venueAmenityRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public class UpdateFieldStatusCommandHandler : IRequestHandler<UpdateFieldStatusCommand, StatusResultDto>
{
    private readonly IFootballFieldRepository _fieldRepository;
    private readonly IVenueRepository _venueRepository;

    public UpdateFieldStatusCommandHandler(IFootballFieldRepository fieldRepository, IVenueRepository venueRepository)
    {
        _fieldRepository = fieldRepository;
        _venueRepository = venueRepository;
    }

    public async Task<StatusResultDto> Handle(UpdateFieldStatusCommand request, CancellationToken cancellationToken)
    {
        var field = await _fieldRepository.GetByIdAsync(request.FieldId, cancellationToken);
        if (field == null)
        {
            throw new NotFoundException(nameof(FootballField), request.FieldId);
        }

        var venue = await _venueRepository.GetByIdAsync(field.VenueId, cancellationToken);
        if (venue == null || venue.OwnerId != request.OwnerId)
        {
            throw new ValidationException("Only the venue owner can update this field.");
        }

        field.IsActive = request.Request.IsActive;
        field.UpdatedAt = DateTime.UtcNow;
        await _fieldRepository.UpdateAsync(field, cancellationToken);
        await _fieldRepository.SaveChangesAsync(cancellationToken);
        return new StatusResultDto { Id = request.FieldId, IsActive = field.IsActive };
    }
}

public class BulkCreateSlotsCommandHandler : IRequestHandler<BulkCreateSlotsCommand, BulkCreateSlotsResultDto>
{
    private readonly IFootballFieldRepository _fieldRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IVenueRepository _venueRepository;

    public BulkCreateSlotsCommandHandler(IFootballFieldRepository fieldRepository, ITimeSlotRepository timeSlotRepository, IVenueRepository venueRepository)
    {
        _fieldRepository = fieldRepository;
        _timeSlotRepository = timeSlotRepository;
        _venueRepository = venueRepository;
    }

    public async Task<BulkCreateSlotsResultDto> Handle(BulkCreateSlotsCommand request, CancellationToken cancellationToken)
    {
        var field = await _fieldRepository.GetByIdAsync(request.FieldId, cancellationToken);
        if (field == null)
        {
            throw new NotFoundException(nameof(FootballField), request.FieldId);
        }

        var venue = await _venueRepository.GetByIdAsync(field.VenueId, cancellationToken);
        if (venue == null || venue.OwnerId != request.OwnerId)
        {
            throw new ValidationException("Only the venue owner can create slots for this field.");
        }

        if (!TimeOnly.TryParse(request.Request.StartTime, out var startTimeOnly) ||
            !TimeOnly.TryParse(request.Request.EndTime, out var endTimeOnly))
        {
            throw new ValidationException("StartTime and EndTime must be valid time values.");
        }

        CreateTimeSlotCommandHandler.ValidateSlot(startTimeOnly, endTimeOnly);

        if (request.Request.SlotDurationMinutes <= 0)
        {
            throw new ValidationException("SlotDurationMinutes must be greater than zero.");
        }

        var windowMinutes = (endTimeOnly - startTimeOnly).TotalMinutes;
        if (request.Request.SlotDurationMinutes > windowMinutes)
        {
            throw new ValidationException(
                "SlotDurationMinutes cannot be greater than the selected time range.");
        }

        if (request.Request.Price <= 0)
        {
            throw new ValidationException("Price must be greater than zero.");
        }

        var proposals = new List<TimeSlot>();
        for (var date = request.Request.FromDate.Date; date <= request.Request.ToDate.Date; date = date.AddDays(1))
        {
            for (var slotStart = startTimeOnly; slotStart.AddMinutes(request.Request.SlotDurationMinutes) <= endTimeOnly; slotStart = slotStart.AddMinutes(request.Request.SlotDurationMinutes))
            {
                var selectedDate = DateOnly.FromDateTime(date);
                proposals.Add(new TimeSlot
                {
                    SlotId = Guid.NewGuid(),
                    FieldId = request.FieldId,
                    StartTime = slotStart,
                    EndTime = slotStart.AddMinutes(request.Request.SlotDurationMinutes),
                    SelectedDate = selectedDate,
                    Price = request.Request.Price,
                    SlotStatus = SlotStatus.Available,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        foreach (var proposal in proposals)
        {
            if (await _timeSlotRepository.HasOverlappingSlotAsync(
                    proposal.FieldId,
                    proposal.SelectedDate,
                    proposal.StartTime,
                    proposal.EndTime,
                    excludedSlotId: null,
                    cancellationToken))
            {
                throw new ValidationException(
                    $"The time slot {proposal.StartTime:HH\\:mm}-{proposal.EndTime:HH\\:mm} on {proposal.SelectedDate:yyyy-MM-dd} overlaps an existing slot for this field.");
            }
        }

        foreach (var proposal in proposals)
        {
            await _timeSlotRepository.AddAsync(proposal, cancellationToken);
        }

        await _timeSlotRepository.SaveChangesAsync(cancellationToken);
        return new BulkCreateSlotsResultDto { CreatedSlots = proposals.Count };
    }
}

public class UpdateSlotStatusCommandHandler : IRequestHandler<UpdateSlotStatusCommand, StatusResultDto>
{
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IFootballFieldRepository _fieldRepository;
    private readonly IVenueRepository _venueRepository;

    public UpdateSlotStatusCommandHandler(ITimeSlotRepository timeSlotRepository, IFootballFieldRepository fieldRepository, IVenueRepository venueRepository)
    {
        _timeSlotRepository = timeSlotRepository;
        _fieldRepository = fieldRepository;
        _venueRepository = venueRepository;
    }

    public async Task<StatusResultDto> Handle(UpdateSlotStatusCommand request, CancellationToken cancellationToken)
    {
        var slot = await _timeSlotRepository.GetByIdAsync(request.SlotId, cancellationToken);
        if (slot == null)
        {
            throw new NotFoundException(nameof(TimeSlot), request.SlotId);
        }

        var field = await _fieldRepository.GetByIdAsync(slot.FieldId, cancellationToken);
        var venue = field != null ? await _venueRepository.GetByIdAsync(field.VenueId, cancellationToken) : null;
        if (venue == null || venue.OwnerId != request.OwnerId)
        {
            throw new ValidationException("Only the venue owner can update this slot.");
        }

        slot.SlotStatus = CreateTimeSlotCommandHandler.ParseSlotStatus(request.Request.SlotStatus, slot.SlotStatus);
        slot.UpdatedAt = DateTime.UtcNow;
        await _timeSlotRepository.UpdateAsync(slot, cancellationToken);
        await _timeSlotRepository.SaveChangesAsync(cancellationToken);
        return new StatusResultDto { Id = request.SlotId, Status = slot.SlotStatus.ToString() };
    }
}

public class CompleteBookingCommandHandler : IRequestHandler<CompleteBookingCommand, StatusResultDto>
{
    private readonly IBookingRepository _bookingRepository;

    public CompleteBookingCommandHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<StatusResultDto> Handle(CompleteBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
        {
            throw new NotFoundException(nameof(Booking), request.BookingId);
        }

        if (!booking.BookingItems.Any(i => i.Slot?.Field?.Venue?.OwnerId == request.OwnerId))
        {
            throw new ValidationException("Only the venue owner can complete this booking.");
        }

        booking.BookingStatus = BookingStatus.Completed;
        booking.UpdatedAt = DateTime.UtcNow;
        foreach (var item in booking.BookingItems)
        {
            if (item.Slot != null)
            {
                item.Slot.SlotStatus = SlotStatus.Booked;
                item.Slot.LockedUntil = null;
            }
        }

        await _bookingRepository.UpdateAsync(booking, cancellationToken);
        await _bookingRepository.SaveChangesAsync(cancellationToken);
        return new StatusResultDto { Id = request.BookingId, Status = booking.BookingStatus.ToString() };
    }
}

public class UpdateDiscountStatusCommandHandler : IRequestHandler<UpdateDiscountStatusCommand, StatusResultDto>
{
    private readonly IDiscountRepository _discountRepository;

    public UpdateDiscountStatusCommandHandler(IDiscountRepository discountRepository)
    {
        _discountRepository = discountRepository;
    }

    public async Task<StatusResultDto> Handle(UpdateDiscountStatusCommand request, CancellationToken cancellationToken)
    {
        var discount = await _discountRepository.GetByIdAsync(request.DiscountId, cancellationToken);
        if (discount == null)
        {
            throw new NotFoundException(nameof(Discount), request.DiscountId);
        }

        if (discount.OwnerId != request.OwnerId)
        {
            throw new ValidationException("Only the discount owner can update this discount.");
        }

        discount.IsActive = request.Request.IsActive;
        await _discountRepository.UpdateAsync(discount, cancellationToken);
        await _discountRepository.SaveChangesAsync(cancellationToken);
        return new StatusResultDto { Id = request.DiscountId, IsActive = discount.IsActive };
    }
}
