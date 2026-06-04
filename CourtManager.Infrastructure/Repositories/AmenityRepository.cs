using CourtManager.Domain.Entities;
using CourtManager.Application.Interfaces;
using CourtManager.Infrastructure.Data;

namespace CourtManager.Infrastructure.Repositories;

public class AmenityRepository : Repository<Amenity>, IAmenityRepository
{
    public AmenityRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
