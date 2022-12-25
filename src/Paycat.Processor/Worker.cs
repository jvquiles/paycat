using Paycat.Infrastructure;

namespace Paycat.Processor;

public class Worker : BackgroundService
{
    private readonly IListener _listener;
    private readonly ILogger<Worker> _logger;

    public Worker(IListener listener, ILogger<Worker> logger)
    {
        _listener = listener;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _listener.Start(stoppingToken);
        return Task.CompletedTask;
    }
}