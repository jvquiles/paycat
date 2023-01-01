using Autofac;
using MediatR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection;

namespace Paycat.Infrastructure.RabbitMQ;

public class RabbitMqReceiver : IReceiver
{
    private readonly IResultStorage _resultStorage;
    private readonly RabbitMqReceiverOptions _rabbitMqReceiverOptions;
    private readonly IModel _channel;

    public RabbitMqReceiver(IResultStorage resultStorage, RabbitMqReceiverOptions rabbitMqReceiverOptions)
    {
        var factory = new ConnectionFactory()
        {
            HostName = rabbitMqReceiverOptions?.Host ?? "localhost",
            Port = rabbitMqReceiverOptions?.Port ?? 5672,
            UserName = rabbitMqReceiverOptions?.User ?? "guest",
            Password = rabbitMqReceiverOptions?.Password ?? "guest",
        };
        _resultStorage = resultStorage;
        _rabbitMqReceiverOptions = rabbitMqReceiverOptions;
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
            var responseQueueName = $"{responseType.Name}_{_rabbitMqReceiverOptions.HostName}";
            _channel.QueueDeclare(
                queue: responseQueueName,
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

            _channel.BasicConsume(
                queue: responseQueueName,
                autoAck: false,
                consumer: consumer);
        }

        return Task.CompletedTask;
    }
}