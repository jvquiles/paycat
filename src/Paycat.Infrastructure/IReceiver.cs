namespace Paycat.Infrastructure;

public interface IReceiver
{
    Task Start(CancellationToken stoppingToken);
}