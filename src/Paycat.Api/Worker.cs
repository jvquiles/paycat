using Paycat.Infrastructure;

namespace Paycat.Api;

public class Worker : BackgroundService
{
    private readonly IReceiver _receiver;
    private readonly ILogger<Worker> _logger;

    public Worker(IReceiver receiver, ILogger<Worker> logger)
    {
        _receiver = receiver;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _receiver.Start(stoppingToken);
        return Task.CompletedTask;
    }
}