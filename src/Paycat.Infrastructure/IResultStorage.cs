using System.Collections.Concurrent;

namespace Paycat.Infrastructure;

public interface IResultStorage
{
    bool TryGetValue(string resultId, out BlockingCollection<byte[]>? blockingCollection);
    bool TryRemove(string resultId, out BlockingCollection<byte[]>? blockingCollection);
    bool TryAdd(string resultId, BlockingCollection<byte[]> blockingCollection, TimeSpan lifeSpan);
}