using Microsoft.EntityFrameworkCore;
using CourtManager.Application.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

/// <summary>
/// Base repository implementation providing common CRUD operations.
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        // Soft delete: Check if entity has IsDeleted property
        var deletedProperty = entity.GetType().GetProperty("IsDeleted");
        var deletedAtProperty = entity.GetType().GetProperty("DeletedAt");

        if (deletedProperty != null && deletedAtProperty != null)
        {
            deletedProperty.SetValue(entity, true);
            deletedAtProperty.SetValue(entity, DateTime.UtcNow);
            _dbSet.Update(entity);
        }
        else
        {
            // Hard delete for entities without soft delete support
            _dbSet.Remove(entity);
        }

        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
