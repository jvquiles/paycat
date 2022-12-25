namespace Paycat.Infrastructure;

public interface IMessenger
{
    Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, TimeSpan timeout)
        where TResponse : class;
}