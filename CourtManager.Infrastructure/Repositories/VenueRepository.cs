using CourtManager.Domain.Entities;
using CourtManager.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourtManager.Infrastructure.Repositories;

public class VenueRepository : Repository<Venue>, IVenueRepository
{
    private readonly ApplicationDbContext _dbContext;

    public VenueRepository(ApplicationDbContext context) : base(context)
    {
        _dbContext = context;
    }

    private IQueryable<Venue> BuildFilterQuery(
        string? q, 
        decimal? priceMin, 
        decimal? priceMax, 
        double? minRating,
        double? userLatitude,
        double? userLongitude,
        double? radiusInKm)
    {
        var query = _dbContext.Venues
            .Include(v => v.Owner)
            .Include(v => v.FootballFields)
            .Include(v => v.Reviews)
            .Where(v => v.IsActive && !v.IsDeleted);

        if (!string.IsNullOrWhiteSpace(q))
        {
            var search = q.ToLower();
            query = query.Where(v => v.VenueName.ToLower().Contains(search) || v.Address.ToLower().Contains(search));
        }

        if (priceMin.HasValue)
        {
            query = query.Where(v => v.FootballFields.Any() && v.FootballFields.Min(f => f.PricePerHour) >= priceMin.Value);
        }

        if (priceMax.HasValue)
        {
            query = query.Where(v => v.FootballFields.Any() && v.FootballFields.Min(f => f.PricePerHour) <= priceMax.Value);
        }

        if (minRating.HasValue)
        {
            query = query.Where(v => v.Reviews.Any() && v.Reviews.Average(r => r.Rating) >= minRating.Value);
        }

        if (userLatitude.HasValue && userLongitude.HasValue && radiusInKm.HasValue)
        {
            double lat = userLatitude.Value;
            double lng = userLongitude.Value;
            double radius = radiusInKm.Value;

            double latDelta = radius / 111.0;
            double lngDelta = radius / (111.0 * Math.Cos(lat * Math.PI / 180.0));

            double minLat = lat - latDelta;
            double maxLat = lat + latDelta;
            double minLng = lng - lngDelta;
            double maxLng = lng + lngDelta;

            query = query.Where(v => v.Latitude >= minLat && v.Latitude <= maxLat
                                  && v.Longitude >= minLng && v.Longitude <= maxLng);
        }

        return query;
    }

    public async Task<IEnumerable<Venue>> GetVenuesAsync(
        string? q, 
        decimal? priceMin, 
        decimal? priceMax, 
        double? minRating, 
        double? userLatitude,
        double? userLongitude,
        double? radiusInKm,
        int skip, 
        int take, 
        CancellationToken cancellationToken = default)
    {
        var query = BuildFilterQuery(q, priceMin, priceMax, minRating, userLatitude, userLongitude, radiusInKm);
        
        if (userLatitude.HasValue && userLongitude.HasValue)
        {
            query = query.OrderBy(v => 
                (v.Latitude - userLatitude.Value) * (v.Latitude - userLatitude.Value) + 
                (v.Longitude - userLongitude.Value) * (v.Longitude - userLongitude.Value)
            );
        }
        else
        {
            query = query.OrderByDescending(v => v.CreatedAt);
        }

        return await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(
        string? q, 
        decimal? priceMin, 
        decimal? priceMax, 
        double? minRating, 
        double? userLatitude,
        double? userLongitude,
        double? radiusInKm,
        CancellationToken cancellationToken = default)
    {
        var query = BuildFilterQuery(q, priceMin, priceMax, minRating, userLatitude, userLongitude, radiusInKm);
        return await query.CountAsync(cancellationToken);
    }



    public async Task<Venue?> GetVenueByIdAsync(Guid venueId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Venues
            .Include(v => v.Owner)
            .Include(v => v.FootballFields)
            .Include(v => v.VenueImages)
            .Include(v => v.VenueAmenities)
                .ThenInclude(va => va.Amenity)
            .Include(v => v.Reviews)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(v => v.VenueId == venueId && v.IsActive && !v.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Amenity>> GetVenueAmenitiesAsync(Guid venueId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.VenueAmenities
            .Where(va => va.VenueId == venueId)
            .Include(va => va.Amenity)
            .Select(va => va.Amenity!)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Venue> BuildOwnerQuery(Guid ownerId, bool? isActive)
    {
        var query = _dbContext.Venues
            .Include(v => v.Owner)
            .Include(v => v.FootballFields)
            .Include(v => v.Reviews)
            .Where(v => v.OwnerId == ownerId && !v.IsDeleted);

        if (isActive.HasValue)
        {
            query = query.Where(v => v.IsActive == isActive.Value);
        }

        return query;
    }

    public async Task<IEnumerable<Venue>> GetOwnerVenuesAsync(
        Guid ownerId,
        bool? isActive,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await BuildOwnerQuery(ownerId, isActive)
            .OrderByDescending(v => v.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetOwnerVenuesCountAsync(
        Guid ownerId,
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        return await BuildOwnerQuery(ownerId, isActive).CountAsync(cancellationToken);
    }

}
