using RealEstate.Entities.Users;
using RealEstate.Entities.Users.Authentications;

namespace RealEstate.Repositories;

#pragma warning disable CA1515 
public interface IBaseRepository<T>
{
    Task<IEnumerable<T>> GetListAsync();
    Task<T?> GetAsync(Guid id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task DeleteAllAsync();
}

public abstract class BaseRepository<T>() : IBaseRepository<T>
{
    public abstract Task<IEnumerable<T>> GetListAsync();

    public abstract Task<T?> GetAsync(Guid id);

    public abstract Task<T> AddAsync(T entity);

    public abstract Task UpdateAsync(T entity);

    public abstract Task DeleteAsync(T entity);

    public abstract Task DeleteAllAsync();
}