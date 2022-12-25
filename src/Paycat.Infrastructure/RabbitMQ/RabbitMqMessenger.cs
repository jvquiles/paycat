using RabbitMQ.Client;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Paycat.Infrastructure.RabbitMQ;

internal class RabbitMqMessenger : IMessenger
{
    private readonly IResultStorage _resultStorage;
    private readonly RabbitMqOptions _rabbitMqOptions;

    public RabbitMqMessenger(IResultStorage resultStorage, RabbitMqOptions rabbitMqOptions)
    {
        _resultStorage = resultStorage;
        _rabbitMqOptions = rabbitMqOptions;
    }

    public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, TimeSpan timeout)
        where TResponse: class
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var correlationId = Guid.NewGuid().ToString();
        _resultStorage.TryAdd(correlationId, new BlockingCollection<byte[]>(), timeout);
        Send(request, correlationId, timeout);

        if (!_resultStorage.TryGetValue(correlationId, out var result) || result is null)
        {
            throw new Exception($"No result expected for message {correlationId} id.");
        }

        result.TryTake(out var bytesRequest, timeout);
        _resultStorage.TryRemove(correlationId, out _);

        if (bytesRequest is null)
        {
            return default;
        }

        var messageResponse = Encoding.UTF8.GetString(bytesRequest);
        var response = JsonSerializer.Deserialize<TResponse>(messageResponse);

        if (response is null)
        {
            return default;
        }

        return await Task.FromResult(response);
    }

    private void Send<TRequest>(TRequest request, string correlationId, TimeSpan timeout)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _rabbitMqOptions.Host,
            Port = _rabbitMqOptions.Port,
            UserName = _rabbitMqOptions.User,
            Password = _rabbitMqOptions.Password,
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: request.GetType().Name,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        var requestMessage = JsonSerializer.Serialize(request);
        var bytesRequest = Encoding.UTF8.GetBytes(requestMessage);

        var basicProperties = channel.CreateBasicProperties();
        basicProperties.CorrelationId = correlationId.ToString();
        basicProperties.Persistent = false;
        basicProperties.Expiration = timeout.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
        channel.BasicPublish(exchange: string.Empty,
            routingKey: request.GetType().Name,
            basicProperties: basicProperties,
            body: bytesRequest);
    }
}