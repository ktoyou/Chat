namespace backend.Repositories.Interfaces;

public interface IRepository<T>
{
    Task AddAsync(T item);

    Task RemoveAsync(T item);

    Task<T?> GetByGuidAsync(Guid guid);

    Task<List<T>> GetAllAsync();
}