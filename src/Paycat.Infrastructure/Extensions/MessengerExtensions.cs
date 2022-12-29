using Microsoft.Extensions.DependencyInjection;
using Paycat.Infrastructure.InMemory;

namespace Paycat.Infrastructure.Extensions;

public class MessengerBuilder
{
    public IServiceCollection ServiceCollection { get; }

    public Type MessengerImplementationType { get; set; }

    public MessengerBuilder(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;
    }
}

public static class MessengerExtensions
{
    public static IServiceCollection AddMessenger(this IServiceCollection serviceCollection, Action<MessengerBuilder> configure)
    {
        var messengerBuilder = new MessengerBuilder(serviceCollection);
        configure(messengerBuilder);

        if (messengerBuilder.MessengerImplementationType is null)
        {
            throw new ArgumentNullException(nameof(messengerBuilder.MessengerImplementationType), "No implementation for IMessenger was found.");
        }

        serviceCollection.AddScoped(typeof(IMessenger), messengerBuilder.MessengerImplementationType!);
        serviceCollection.AddSingleton<IResultStorage, ResultStorage>();

        return serviceCollection;
    }
}