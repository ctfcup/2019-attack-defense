using System.Collections.Concurrent;

namespace medlink.Storage
{
    public interface ISeriesIndex : IFileBasedIndex<ConcurrentDictionary<string, byte>, string>
    {
    }
}