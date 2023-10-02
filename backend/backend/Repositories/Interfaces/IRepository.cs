namespace backend.Repositories.Interfaces;

public interface IRepository<T>
{
    Task Add(T item);

    Task Remove(T item);

    Task<T> GetByGuid(Guid guid);

    Task<List<T>> GetAll();
}