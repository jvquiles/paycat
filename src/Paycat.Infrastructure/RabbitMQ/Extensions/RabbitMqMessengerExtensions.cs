using Microsoft.Extensions.DependencyInjection;
using Paycat.Infrastructure.Extensions;

namespace Paycat.Infrastructure.RabbitMQ.Extensions;

public static class RabbitMqMessengerExtensions
{
    public class RabbitMqMessengerBuilder
    {
        public RabbitMqMessengerOptions Options { get; set; }
    }

    public static MessengerBuilder AddRabbitMq(this MessengerBuilder messengerBuilder, Action<RabbitMqMessengerBuilder> configure)
    {
        var builder = new RabbitMqMessengerBuilder();
        configure(builder);

        if (builder.Options is null)
        {
            throw new ArgumentNullException(nameof(builder.Options));
        }

        messengerBuilder.ServiceCollection.AddSingleton<RabbitMqMessengerOptions>(builder.Options);
        messengerBuilder.MessengerImplementationType = typeof(RabbitMqMessenger);

        return messengerBuilder;
    }
}