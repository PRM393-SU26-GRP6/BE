using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;
using CourtManager.Application.Interfaces;
using CourtManager.Application.Exceptions;

namespace CourtManager.Infrastructure.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    private readonly ApplicationDbContext _db;

    public BookingRepository(ApplicationDbContext context) : base(context)
    {
        _db = context;
    }

    public override async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await IncludeDetails(_dbSet)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await IncludeDetails(_dbSet)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetBookingsByCourtIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await IncludeDetails(_dbSet)
            .Where(b => b.BookingItems.Any(i => i.Slot != null && i.Slot.FieldId == fieldId))
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetPendingBookingsForOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await IncludeDetails(_dbSet)
            .Where(b => b.BookingStatus == CourtManager.Domain.Enums.BookingStatus.Pending
                && b.BookingItems.Any(i => i.Slot != null && i.Slot.Field != null && i.Slot.Field.Venue != null && i.Slot.Field.Venue.OwnerId == ownerId))
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetBookingsForOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await IncludeDetails(_dbSet)
            .Where(b => b.BookingItems.Any(i => i.Slot != null && i.Slot.Field != null && i.Slot.Field.Venue != null && i.Slot.Field.Venue.OwnerId == ownerId))
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsCourtAvailableAsync(Guid fieldId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        var startDate = DateOnly.FromDateTime(startTime);
        var endDate = DateOnly.FromDateTime(endTime);
        var startTimeOnly = TimeOnly.FromDateTime(startTime);
        var endTimeOnly = TimeOnly.FromDateTime(endTime);

        return !await _context.TimeSlots
            .AnyAsync(s => s.FieldId == fieldId
                && s.SelectedDate >= startDate
                && s.SelectedDate <= endDate
                && s.StartTime < endTimeOnly
                && s.EndTime > startTimeOnly
                && s.SlotStatus != CourtManager.Domain.Enums.SlotStatus.Available,
                cancellationToken);
    }

    private static IQueryable<Booking> IncludeDetails(IQueryable<Booking> query)
    {
        return query
            .Include(b => b.User)
            .Include(b => b.Payments)
            .Include(b => b.BookingDiscounts)
                .ThenInclude(bd => bd.Discount)
            .Include(b => b.BookingItems)
                .ThenInclude(i => i.Slot!)
                    .ThenInclude(s => s.Field!)
                        .ThenInclude(f => f.Venue);
    }

    public async Task<bool> HasActiveBookingsForVenueAsync(Guid venueId, CancellationToken cancellationToken = default)
    {
        // Active = Pending, Accepted, or Deposited (still in-flight, not yet completed/rejected/cancelled)
        var activeStatuses = new[] { BookingStatus.Pending, BookingStatus.Accepted, BookingStatus.Deposited };

        return await _db.Bookings
            .Where(b => !b.IsDeleted && activeStatuses.Contains(b.BookingStatus))
            .AnyAsync(b => b.BookingItems.Any(bi =>
                bi.Slot != null &&
                bi.Slot.Field != null &&
                bi.Slot.Field.VenueId == venueId),
            cancellationToken);
    }

    public async Task<bool> IsBookingValidForReviewAsync(Guid bookingId, Guid venueId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _db.Bookings
            .Where(b => b.Id == bookingId && b.UserId == userId && b.BookingStatus == BookingStatus.Completed && !b.IsDeleted)
            .AnyAsync(b => b.BookingItems.Any(bi =>
                bi.Slot != null &&
                bi.Slot.Field != null &&
                bi.Slot.Field.VenueId == venueId),
            cancellationToken);
    }

    public async Task<Booking?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await IncludeDetails(_db.Bookings)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<bool> AcceptBookingAtomicAsync(Guid bookingId, Guid ownerId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var booking = await IncludeDetails(_db.Bookings)
                .FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);

            if (booking == null)
                throw new NotFoundException(nameof(Booking), bookingId);

            if (ownerId != Guid.Empty && !booking.BookingItems.Any(i => i.Slot?.Field?.Venue?.OwnerId == ownerId))
                throw new ValidationException("Only the owner of the booked venue can accept this booking.");

            if (booking.BookingStatus != BookingStatus.Pending && booking.BookingStatus != BookingStatus.Deposited)
                throw new ValidationException($"Cannot accept booking. Current status is '{booking.BookingStatus}'. Only 'Pending' or 'Deposited' bookings can be accepted.");

            // 1. Update Booking Status
            booking.BookingStatus = BookingStatus.Accepted;
            booking.UpdatedAt = DateTime.UtcNow;

            // 2. Update Slot Statuses to Booked
            var bookedSlotIds = new List<Guid>();
            foreach (var item in booking.BookingItems)
            {
                if (item.Slot != null)
                {
                    item.Slot.SlotStatus = SlotStatus.Booked;
                    bookedSlotIds.Add(item.SlotId);
                }
            }

            // 3. Find overlapping Pending bookings and reject them
            if (bookedSlotIds.Any())
            {
                var overlappingBookings = await _db.Bookings
                    .Include(b => b.BookingItems)
                    .Where(b => b.Id != bookingId 
                             && (b.BookingStatus == BookingStatus.Pending || b.BookingStatus == BookingStatus.Deposited)
                             && !b.IsDeleted)
                    .ToListAsync(cancellationToken);

                // Filter in memory to avoid complex query translation issues
                var conflictingBookings = overlappingBookings
                    .Where(b => b.BookingItems.Any(i => bookedSlotIds.Contains(i.SlotId)))
                    .ToList();

                foreach (var ob in conflictingBookings)
                {
                    ob.BookingStatus = BookingStatus.Rejected;
                    ob.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
