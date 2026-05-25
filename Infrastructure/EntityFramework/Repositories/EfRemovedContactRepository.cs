using AppCore.Dto;
using AppCore.Enums;
using AppCore.Models;
using AppCore.Repositories;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;
using RemovedContactEntity = Infrastructure.EntityFramework.Entities.RemovedContact;

namespace Infrastructure.EntityFramework.Repositories;

public class EfRemovedContactRepository : IRemovedContactRepository
{
    private readonly ContactsDbContext _context;
    private readonly DbSet<RemovedContactEntity> _dbSet;

    public EfRemovedContactRepository(ContactsDbContext context)
    {
        _context = context;
        _dbSet = _context.RemovedContacts;
    }

    public async Task<RemovedContact?> FindByIdAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null) return null;
        
        return MapToAppCore(entity);
    }

    public async Task<IEnumerable<RemovedContact>> FindAllAsync()
    {
        var entities = await _dbSet.ToListAsync();
        return entities.Select(MapToAppCore);
    }

    public async Task<PagedResult<RemovedContact>> FindPagedAsync(int page, int pageSize)
    {
        var entities = await _dbSet
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        var totalCount = await _dbSet.CountAsync();
        var items = entities.Select(MapToAppCore).ToList();
        
        return new PagedResult<RemovedContact>(items, totalCount, page, pageSize);
    }

    public async Task<RemovedContact> AddAsync(RemovedContact entity)
    {
        var dbEntity = new RemovedContactEntity
        {
            Id = entity.Id,
            OriginalId = entity.OriginalId,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            Phone = entity.Phone,
            Address = entity.Address,
            RemovedByUserId = entity.RemovedByUserId,
            RemovedByUserEmail = entity.RemovedByUserEmail,
            RemovedAt = entity.RemovedAt,
            DeduplicationReason = entity.DeduplicationReason,
            DeduplicationStrategy = entity.DeduplicationStrategy,
            Status = entity.Status
        };
        
        var entry = await _dbSet.AddAsync(dbEntity);
        await _context.SaveChangesAsync();
        return MapToAppCore(entry.Entity);
    }

    public async Task<RemovedContact> UpdateAsync(RemovedContact entity)
    {
        var dbEntity = await _dbSet.FindAsync(entity.Id);
        if (dbEntity != null)
        {
            dbEntity.FirstName = entity.FirstName;
            dbEntity.LastName = entity.LastName;
            dbEntity.Email = entity.Email;
            dbEntity.Phone = entity.Phone;
            dbEntity.Address = entity.Address;
            dbEntity.RemovedByUserId = entity.RemovedByUserId;
            dbEntity.RemovedByUserEmail = entity.RemovedByUserEmail;
            dbEntity.RemovedAt = entity.RemovedAt;
            dbEntity.DeduplicationReason = entity.DeduplicationReason;
            dbEntity.DeduplicationStrategy = entity.DeduplicationStrategy;
            dbEntity.Status = entity.Status;
            
            _dbSet.Update(dbEntity);
            await _context.SaveChangesAsync();
        }
        return entity;
    }

    public async Task RemoveByIdAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<RemovedContactDto>> GetByUserIdAsync(string userId)
    {
        var entities = await _dbSet
            .Where(r => r.RemovedByUserId == userId)
            .OrderByDescending(r => r.RemovedAt)
            .ToListAsync();

        return entities.Select(e => new RemovedContactDto
        {
            Id = e.Id,
            OriginalId = e.OriginalId,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            RemovedByUserId = e.RemovedByUserId,
            RemovedByUserEmail = e.RemovedByUserEmail,
            RemovedAt = e.RemovedAt,
            DeduplicationReason = e.DeduplicationReason,
            DeduplicationStrategy = e.DeduplicationStrategy
        }).ToList();
    }

    public async Task<List<RemovedContactDto>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        var entities = await _dbSet
            .Where(r => r.RemovedAt >= from && r.RemovedAt <= to)
            .OrderByDescending(r => r.RemovedAt)
            .ToListAsync();

        return entities.Select(e => new RemovedContactDto
        {
            Id = e.Id,
            OriginalId = e.OriginalId,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            RemovedByUserId = e.RemovedByUserId,
            RemovedByUserEmail = e.RemovedByUserEmail,
            RemovedAt = e.RemovedAt,
            DeduplicationReason = e.DeduplicationReason,
            DeduplicationStrategy = e.DeduplicationStrategy
        }).ToList();
    }

    private RemovedContact MapToAppCore(RemovedContactEntity entity)
    {
        return new RemovedContact
        {
            Id = entity.Id,
            OriginalId = entity.OriginalId,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            Phone = entity.Phone,
            Address = entity.Address,
            RemovedByUserId = entity.RemovedByUserId,
            RemovedByUserEmail = entity.RemovedByUserEmail,
            RemovedAt = entity.RemovedAt,
            DeduplicationReason = entity.DeduplicationReason,
            DeduplicationStrategy = entity.DeduplicationStrategy,
            Status = entity.Status
        };
    }
}