namespace Paycat.Infrastructure;

public interface IRepository<TEntity>
    where TEntity: class
{
    Task<TEntity> AddOrUpdate(Guid id, TEntity entity);
    Task Remove(Guid id);
    Task<TEntity> Get(Guid id);
}