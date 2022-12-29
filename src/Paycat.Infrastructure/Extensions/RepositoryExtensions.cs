using Microsoft.Extensions.DependencyInjection;

namespace Paycat.Infrastructure.Extensions;

public class RepositoryBuilder
{
    public IServiceCollection ServiceCollection { get; }

    public Type RepositoryType { get; set; }

    public RepositoryBuilder(IServiceCollection servicesCollection)
    {
        ServiceCollection = servicesCollection;
    }
}

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepository(this IServiceCollection serviceCollection, Action<RepositoryBuilder> config)
    {
        var builder = new RepositoryBuilder(serviceCollection);
        config(builder);

        if (builder.RepositoryType is null)
        {
            throw new ArgumentNullException(nameof(builder.RepositoryType), "No implementation for IRepository was found.");
        }
        
        serviceCollection.AddScoped(typeof(IRepository<>), builder.RepositoryType);

        return serviceCollection;
    }
}