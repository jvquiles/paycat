namespace Paycat.Infrastructure.RabbitMQ;

public class RabbitMqMessengerOptions
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public string HostName { get; set; }
}