using System.Collections.Generic;
using medlink.Helpers;

namespace medlink.Storage
{
    public class SeriesIndex : FileBasedIndex<HashSet<string>, string>, ISeriesIndex
    {
        public SeriesIndex(IFileDumper fileDumper, ISettings settings) : base(fileDumper, settings, settings.SeriesIndex)
        {
        }
    }
}