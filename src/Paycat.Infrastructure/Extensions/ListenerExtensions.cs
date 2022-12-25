using Microsoft.Extensions.DependencyInjection;
using Paycat.Infrastructure.InMemory;

namespace Paycat.Infrastructure.Extensions;

public class ListenerBuilder
{
    public IServiceCollection ServiceCollection { get; }

    public Type? ListenerImplementationType { get; set; }

    public ListenerBuilder(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;
    }
}

public static class ListenerExtensions
{
    public static IServiceCollection AddListener(this IServiceCollection serviceCollection, Action<ListenerBuilder> configure)
    {
        var listenerBuilder = new ListenerBuilder(serviceCollection);
        configure(listenerBuilder);

        if (listenerBuilder.ListenerImplementationType is null)
        {
            throw new ArgumentNullException(nameof(listenerBuilder.ListenerImplementationType), "No implementation for IListener was found.");
        }

        serviceCollection.AddSingleton(typeof(IListener), listenerBuilder.ListenerImplementationType!);
        serviceCollection.AddSingleton<IResultStorage, ResultStorage>();

        return serviceCollection;
    }
}