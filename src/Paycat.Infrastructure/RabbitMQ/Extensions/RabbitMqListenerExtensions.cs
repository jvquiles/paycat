using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Paycat.Infrastructure.Extensions;
using System.Reflection;

namespace Paycat.Infrastructure.RabbitMQ.Extensions;

public static class RabbitMqListenerExtensions
{
    public class RabbitMqListenerBuilder
    {
        public RabbitMqOptions Options { get; set; }
    }

    public static ListenerBuilder AddRabbitMq(this ListenerBuilder listenerBuilder, Action<RabbitMqListenerBuilder> configure, params Assembly[]? assemblies)
    {
        var builder = new RabbitMqListenerBuilder();
        configure(builder);

        if (builder.Options is null)
        {
            throw new ArgumentNullException(nameof(builder.Options));
        }

        listenerBuilder.ServiceCollection.AddSingleton<RabbitMqOptions>(builder.Options);
        listenerBuilder.ListenerImplementationType = typeof(RabbitMqListener);
        listenerBuilder.ServiceCollection.AddMediatR(assemblies ?? new Assembly[] { Assembly.GetExecutingAssembly() });

        return listenerBuilder;
    }
}