using Microsoft.Extensions.DependencyInjection;
using Paycat.Infrastructure.Extensions;

namespace Paycat.Infrastructure.Redis.Extensions;

public static class RedisRepositoryExtensions
{
    public class RedisRepositoryBuilder
    {
        public RedisRepositoryOptions Options { get; set; }
    }

    public static RepositoryBuilder AddRedisRepository(this RepositoryBuilder repositoryBuilder, Action<RedisRepositoryBuilder> configure)
    {
        var builder = new RedisRepositoryBuilder();
        configure(builder);

        if (builder.Options is null)
        {
            throw new ArgumentNullException(nameof(builder.Options));
        }

        repositoryBuilder.ServiceCollection.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Options.ConnectionString;
        });

        repositoryBuilder.RepositoryType = typeof(RedisRepository<>);
        return repositoryBuilder;
    }
}