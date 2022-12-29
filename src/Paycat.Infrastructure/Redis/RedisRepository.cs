using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Paycat.Infrastructure.Redis;

internal class RedisRepository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    private readonly IDistributedCache _redisCache;

    public RedisRepository(IDistributedCache redisCache)
    {
        _redisCache = redisCache;
    }

    public async Task<TEntity> AddOrUpdate(Guid id, TEntity entity)
    {
        var data = JsonSerializer.Serialize(entity);
        await _redisCache.SetStringAsync(id.ToString(), data);
        return entity;
    }

    public Task Remove(Guid id)
    {
        return _redisCache.RemoveAsync(id.ToString());
    }

    public async Task<TEntity> Get(Guid id)
    {
        var data = await _redisCache.GetStringAsync(id.ToString());
        if (data is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<TEntity>(data);
    }
}