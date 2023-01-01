using Autofac;
using MediatR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Paycat.Infrastructure.RabbitMQ;

public class RabbitMqListener : IListener
{
    private readonly IMediator _mediator;
    private readonly RabbitMqListenerOptions _rabbitMqListenerOptions;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqListener(IMediator mediator, RabbitMqListenerOptions rabbitMqListenerOptions)
    {
        _mediator = mediator;
        _rabbitMqListenerOptions = rabbitMqListenerOptions;
        var factory = new ConnectionFactory()
        {
            HostName = rabbitMqListenerOptions?.Host ?? "localhost",
            Port = rabbitMqListenerOptions?.Port ?? 5672,
            UserName = rabbitMqListenerOptions?.User ?? "guest",
            Password = rabbitMqListenerOptions?.Password ?? "guest",
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public Task Start(CancellationToken stoppingToken)
    {
        var requestHandlerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes().Where(t => t.IsClosedTypeOf(typeof(IRequestHandler<,>))))
            .Select(t =>
            {
                var requestHandlerGenericTypes = t.GetInterfaces().First().GenericTypeArguments;
                return (RequestType: requestHandlerGenericTypes.First(), ResponseType: requestHandlerGenericTypes.Last().GetGenericArguments().First());
            })
            .ToArray();

        foreach (var (requestType, responseType) in requestHandlerTypes)
        {
            Listen(requestType, responseType, stoppingToken);
        }

        return Task.CompletedTask;
    }

    private void Listen(Type requestType, Type responseType, CancellationToken stoppingToken)
    {
        _channel.QueueDeclare(
            queue: requestType.Name,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, args) =>
        {
            var bytesRequest = args.Body.ToArray();
            var messageResponse = Encoding.UTF8.GetString(bytesRequest);
            var request = JsonSerializer.Deserialize(messageResponse, requestType);
            if (request is null)
            {
                return;
            }

            using var outputChannel = _connection.CreateModel();
            var queueOutputName = $"{responseType.Name}_{Encoding.UTF8.GetString((byte[])args.BasicProperties.Headers["HOSTNAME"])}";
            outputChannel.QueueDeclare(
                queue: queueOutputName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var response = await _mediator.Send(request, stoppingToken)
                .ConfigureAwait(false);

            var processedTransactionMessage = JsonSerializer.Serialize(response);
            var bytesResponse = Encoding.UTF8.GetBytes(processedTransactionMessage);

            var basicProperties = _channel.CreateBasicProperties();
            basicProperties.CorrelationId = args.BasicProperties.CorrelationId;
            basicProperties.Persistent = false;
            basicProperties.Expiration = args.BasicProperties.Expiration;
            outputChannel.BasicPublish(
                exchange: string.Empty,
                routingKey: queueOutputName,
                basicProperties: basicProperties,
                body: bytesResponse);
        };

        _channel.BasicConsume(
            queue: requestType.Name,
            autoAck: false,
            consumer: consumer);
    }
}