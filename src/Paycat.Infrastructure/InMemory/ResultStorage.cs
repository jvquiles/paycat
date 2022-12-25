using System.Collections.Concurrent;

namespace Paycat.Infrastructure.InMemory;

public class ResultStorage : IResultStorage
{
    private readonly ConcurrentDictionary<string, BlockingCollection<byte[]>?> _results;

    public ResultStorage()
    {
        _results = new ConcurrentDictionary<string, BlockingCollection<byte[]>?>();
    }

    public bool TryGetValue(string resultId, out BlockingCollection<byte[]>? blockingCollection)
    {
        return _results.TryGetValue(resultId, out blockingCollection);
    }

    public bool TryRemove(string resultId, out BlockingCollection<byte[]>? blockingCollection)
    {
        return _results.TryRemove(resultId, out blockingCollection);
    }

    public bool TryAdd(string resultId, BlockingCollection<byte[]> blockingCollection, TimeSpan _)
    {
        return _results.TryAdd(resultId, blockingCollection);
    }
}