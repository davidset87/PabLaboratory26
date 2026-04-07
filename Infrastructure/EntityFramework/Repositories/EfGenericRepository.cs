using AppCore.Dto;
using AppCore.Models;
using AppCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfGenericRepository<T> : IGenericRepositoryAsync<T> where T : EntityBase
{
    protected readonly DbSet<T> _dbSet;

    public EfGenericRepository(DbSet<T> dbSet)
    {
        _dbSet = dbSet;
    }

    public async Task<T?> FindByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> FindAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<PagedResult<T>> FindPagedAsync(int page, int pageSize)
    {
        var items = await _dbSet
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalCount = await _dbSet.CountAsync();

        return new PagedResult<T>(items, totalCount, page, pageSize);
    }

    public async Task<T> AddAsync(T entity)
    {
        var entry = await _dbSet.AddAsync(entity);
        return entry.Entity;
    }

    public Task<T> UpdateAsync(T entity)
    {
        var entry = _dbSet.Update(entity);
        return Task.FromResult(entry.Entity);
    }

    public async Task RemoveByIdAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity is not null)
        {
            _dbSet.Remove(entity);
        }
    }
}