using Microsoft.Extensions.DependencyInjection;
using Paycat.Infrastructure.InMemory;

namespace Paycat.Infrastructure.Extensions;

public class ReceiverBuilder
{
    public IServiceCollection ServiceCollection { get; }

    public Type ReceiverImplementationType { get; set; }

    public ReceiverBuilder(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;
    }
}

public static class ReceiverExtensions
{
    public static IServiceCollection AddReceiver(this IServiceCollection serviceCollection, Action<ReceiverBuilder> configure)
    {
        var receiverBuilder = new ReceiverBuilder(serviceCollection);
        configure(receiverBuilder);

        if (receiverBuilder.ReceiverImplementationType is null)
        {
            throw new ArgumentNullException(nameof(receiverBuilder.ReceiverImplementationType), "No implementation for IReceiver was found.");
        }

        serviceCollection.AddSingleton(typeof(IReceiver), receiverBuilder.ReceiverImplementationType!);
        serviceCollection.AddSingleton<IResultStorage, ResultStorage>();

        return serviceCollection;
    }
}