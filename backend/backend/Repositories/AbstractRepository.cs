using backend.Db;
using backend.Repositories.Interfaces;

namespace backend.Repositories;

public abstract class AbstractRepository<T> : IRepository<T>
{
    protected DbApplicationContext _dbApplicationContext;

    protected AbstractRepository(DbApplicationContext context)
    {
        _dbApplicationContext = context;
    }
    
    public abstract Task Add(T item);

    public abstract Task Remove(T item);

    public abstract Task<T> GetByGuid(Guid guid);

    public abstract Task<List<T>> GetAll();
}