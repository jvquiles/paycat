using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Paycat.Infrastructure.Extensions;

namespace Paycat.Infrastructure.RabbitMQ.Extensions;

public static class RabbitMqMessengerExtensions
{
    public class RabbitMqMessengerBuilder
    {
        public RabbitMqOptions? Options { get; set; }
    }

    public static MessengerBuilder AddRabbitMq(this MessengerBuilder messengerBuilder, Action<RabbitMqMessengerBuilder> configure)
    {
        var builder = new RabbitMqMessengerBuilder();
        configure(builder);

        if (builder.Options is null)
        {
            throw new ArgumentNullException(nameof(builder.Options));
        }

        messengerBuilder.ServiceCollection.Configure<RabbitMqOptions>(rabbitMqOptions =>
        {
            rabbitMqOptions = builder.Options;
        });

        messengerBuilder.ServiceCollection.AddSingleton<RabbitMqOptions>(builder.Options);
        messengerBuilder.MessengerImplementationType = typeof(RabbitMqMessenger);

        return messengerBuilder;
    }
}