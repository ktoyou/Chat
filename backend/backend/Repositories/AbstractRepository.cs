using backend.Db;
using backend.Repositories.Interfaces;

namespace backend.Repositories;

public abstract class AbstractRepository<T> : IRepository<T>
{
    protected readonly DbApplicationContext _dbApplicationContext;

    protected AbstractRepository(DbApplicationContext context)
    {
        _dbApplicationContext = context;
    }
    
    public abstract Task AddAsync(T item);

    public abstract Task RemoveAsync(T item);

    public abstract Task<T?> GetByGuidAsync(Guid guid);

    public abstract Task<List<T>> GetAllAsync();
}