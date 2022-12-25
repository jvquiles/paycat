namespace Paycat.Infrastructure;

public interface IListener
{
    Task Start(CancellationToken stoppingToken);
}