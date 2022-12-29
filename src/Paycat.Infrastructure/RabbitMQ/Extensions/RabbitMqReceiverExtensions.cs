using Microsoft.Extensions.DependencyInjection;
using Paycat.Infrastructure.Extensions;

namespace Paycat.Infrastructure.RabbitMQ.Extensions;

public static class RabbitMqReceiverExtensions
{
    public class RabbitMqReceiverBuilder
    {
        public RabbitMqOptions Options { get; set; }
    }

    public static ReceiverBuilder AddRabbitMq(this ReceiverBuilder receiverBuilder, Action<RabbitMqReceiverBuilder> configure)
    {
        var builder = new RabbitMqReceiverBuilder();
        configure(builder);

        if (builder.Options is null)
        {
            throw new ArgumentNullException(nameof(builder.Options));
        }

        receiverBuilder.ServiceCollection.AddSingleton<RabbitMqOptions>(builder.Options);
        receiverBuilder.ReceiverImplementationType = typeof(RabbitMqReceiver);

        return receiverBuilder;
    }
}