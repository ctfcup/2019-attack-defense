using System.Collections.Generic;

namespace medlink.Storage
{
    public interface ISeriesIndex : IFileBasedIndex<HashSet<string>, string>
    {
    }
}