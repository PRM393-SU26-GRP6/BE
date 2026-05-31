using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;
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
        double? minRating)
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

        return query;
    }

    public async Task<IEnumerable<Venue>> GetVenuesAsync(
        string? q, 
        decimal? priceMin, 
        decimal? priceMax, 
        double? minRating, 
        int skip, 
        int take, 
        CancellationToken cancellationToken = default)
    {
        var query = BuildFilterQuery(q, priceMin, priceMax, minRating);
        
        return await query
            .OrderByDescending(v => v.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(
        string? q, 
        decimal? priceMin, 
        decimal? priceMax, 
        double? minRating, 
        CancellationToken cancellationToken = default)
    {
        var query = BuildFilterQuery(q, priceMin, priceMax, minRating);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IEnumerable<(Venue Venue, double Distance)>> GetNearbyVenuesAsync(
        double latitude,
        double longitude,
        double radiusInKm,
        CancellationToken cancellationToken = default)
    {
        // Simple bounding box for SQL performance (1 deg lat ~ 111km, 1 deg lng ~ 111km at equator)
        decimal latOffset = (decimal)(radiusInKm / 111.0);
        decimal lngOffset = (decimal)(radiusInKm / (111.0 * Math.Cos(latitude * Math.PI / 180.0)));

        decimal minLat = (decimal)latitude - latOffset;
        decimal maxLat = (decimal)latitude + latOffset;
        decimal minLng = (decimal)longitude - lngOffset;
        decimal maxLng = (decimal)longitude + lngOffset;

        var venues = await _dbContext.Venues
            .Include(v => v.Owner)
            .Include(v => v.FootballFields)
            .Include(v => v.Reviews)
            .Where(v => v.IsActive && !v.IsDeleted &&
                        v.Latitude >= minLat && v.Latitude <= maxLat &&
                        v.Longitude >= minLng && v.Longitude <= maxLng)
            .ToListAsync(cancellationToken);

        // Precise Haversine distance in memory
        return venues
            .Select(v => (Venue: v, Distance: CalculateDistance(latitude, longitude, (double)v.Latitude, (double)v.Longitude)))
            .Where(x => x.Distance <= radiusInKm)
            .OrderBy(x => x.Distance)
            .ToList();
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

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371; // Radius of the earth in km
        var dLat = Deg2Rad(lat2 - lat1);
        var dLon = Deg2Rad(lon2 - lon1);
        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var d = R * c; // Distance in km
        return d;
    }

    private double Deg2Rad(double deg)
    {
        return deg * (Math.PI / 180);
    }
}
