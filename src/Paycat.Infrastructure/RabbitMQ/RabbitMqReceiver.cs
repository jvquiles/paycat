using System.Reflection;
using Autofac;
using MediatR;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Paycat.Infrastructure.RabbitMQ;

public class RabbitMqReceiver : IReceiver
{
    private readonly IResultStorage _resultStorage;
    private readonly IModel _channel;

    public RabbitMqReceiver(IResultStorage resultStorage, RabbitMqOptions rabbitMqOptions)
    {
        var factory = new ConnectionFactory()
        {
            HostName = rabbitMqOptions?.Host ?? "localhost",
            Port = rabbitMqOptions?.Port ?? 5672,
            UserName = rabbitMqOptions?.User ?? "guest",
            Password = rabbitMqOptions?.Password ?? "guest",
        };
        _resultStorage = resultStorage;
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
    }

    public Task Start(CancellationToken stoppingToken)
    {
        var requestHandlerResponseTypes = Assembly.GetEntryAssembly()?
            .GetTypes()
            .Where(t => t.IsClosedTypeOf(typeof(IRequestHandler<,>)))
            .Select(t =>
            {
                var requestHandlerGenericTypes = t.GetInterfaces().First().GenericTypeArguments;
                return requestHandlerGenericTypes.Last().GetGenericArguments().First();
            })
            .ToArray();

        if (requestHandlerResponseTypes is null)
        {
            return Task.CompletedTask;
        }

        foreach (var responseType in requestHandlerResponseTypes)
        {
            _channel.QueueDeclare(queue: responseType.Name,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (_, args) =>
            {
                var correlationId = args.BasicProperties.CorrelationId;
                if (_resultStorage.TryGetValue(correlationId, out var result) && result is not null)
                {
                    result.Add(args.Body.ToArray(), stoppingToken);
                }
            };

            _channel.BasicConsume(queue: responseType.Name,
                autoAck: false,
                consumer: consumer);
        }

        return Task.CompletedTask;
    }
}