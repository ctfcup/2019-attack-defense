using System.Collections.Generic;
using System.IO;
using System.Linq;
using medlink.Helpers;

namespace medlink.Storage
{
    public static class PathHelper
    {
        public static string BodyModelPath(string series, string revision)
        {
            return Path.Combine(series, revision);
        }
    }
    
    public class SeriesIndexRecord
    {
        public string Series { get; set; }
        public string Revision { get; set; }
    }
    
    public interface ISeriesIndex : IFileBasedIndex<SeriesIndexRecord, string>
    {
    }

    public class SeriesIndex : FileBasedIndex<SeriesIndexRecord, string>, ISeriesIndex
    {
        public SeriesIndex(IFileDumper fileDumper, ISettings settings) : base(fileDumper, settings.SeriesIndex)
        {
        }
    }

    public class BodyDiagnosticStorage : BaseStorage<BodyDiagnosticInfo>, IBodyModelsStorage
    {
        private readonly ISeriesIndex _seriesIndex;
        
        public BodyDiagnosticStorage(ISerializer serializer, ISettings settings, ISeriesIndex seriesIndex) : base(serializer, settings, settings.BodyDiagnosticFolder)
        {
            _seriesIndex = seriesIndex;
        }

        protected override void OnAdd(BodyDiagnosticInfo info, string filename)
        {
            _seriesIndex.Add($"{info.ModelSeries}{info.Revision}", new SeriesIndexRecord
            {
                Revision = info.Revision,
                Series = info.ModelSeries
            });
        }

        public bool Contains(string filename)
        {
            return File.Exists(filename);
        }
        
        public IEnumerable<SeriesIndexRecord> GetAllModels()
        {
            return _seriesIndex.Values;
        }
    }
}