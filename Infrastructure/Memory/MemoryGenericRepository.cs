using AppCore.Dto;
using AppCore.Repositories;

namespace Infrastructure.Memory;

public class MemoryGenericRepository<T> : IGenericRepositoryAsync<T> where T : class
{
    
    protected readonly Dictionary<Guid, T> _data = new();
    private readonly Func<T, Guid> _getId;
    private readonly Action<T, Guid> _setId;

    public MemoryGenericRepository()
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty != null && idProperty.PropertyType == typeof(Guid))
        {
            _getId = entity => (Guid)(idProperty.GetValue(entity) ?? Guid.Empty);
            _setId = (entity, id) => idProperty.SetValue(entity, id);
        }
        else
        {
            throw new InvalidOperationException($"Type {typeof(T).Name} must have a Guid property named 'Id'");
        }
    }

    public virtual Task<T?> FindByIdAsync(Guid id)
    {
        var result = _data.TryGetValue(id, out var value) ? value : null;
        return Task.FromResult(result);
    }

    public virtual Task<IEnumerable<T>> FindAllAsync()
    {
        return Task.FromResult(_data.Values.AsEnumerable());
    }

    public virtual Task<PagedResult<T>> FindPagedAsync(int page, int pageSize)
    {
        var items = _data.Values
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        var totalCount = _data.Count;
        var result = new PagedResult<T>(items, totalCount, page, pageSize);
        
        return Task.FromResult(result);
    }

    public virtual Task<T> AddAsync(T entity)
    {
        var id = Guid.NewGuid();
        _setId(entity, id);
        _data[id] = entity;
        return Task.FromResult(entity);
    }

    public virtual Task<T> UpdateAsync(T entity)
    {
        var id = _getId(entity);
        if (!_data.ContainsKey(id))
        {
            throw new KeyNotFoundException($"Entity with id {id} not found");
        }
        _data[id] = entity;
        return Task.FromResult(entity);
    }

    public virtual Task RemoveByIdAsync(Guid id)
    {
        if (!_data.ContainsKey(id))
        {
            throw new KeyNotFoundException($"Entity with id {id} not found");
        }
        _data.Remove(id);
        return Task.CompletedTask;
    }
}