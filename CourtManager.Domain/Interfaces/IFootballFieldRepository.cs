using CourtManager.Domain.Entities;

namespace CourtManager.Domain.Interfaces;

/// <summary>
/// Repository interface for FootballField entity with specific queries.
/// </summary>
public interface IFootballFieldRepository : IRepository<FootballField>
{
    Task<IEnumerable<FootballField>> GetAvailableFieldsAsync(CancellationToken cancellationToken = default);
    Task<FootballField?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<FootballField>> GetFieldsByVenueIdAsync(Guid venueId, CancellationToken cancellationToken = default);
}
