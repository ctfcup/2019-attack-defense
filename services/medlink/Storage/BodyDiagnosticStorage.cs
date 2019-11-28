using System.Collections.Generic;
using System.IO;
using System.Linq;
using medlink.Helpers;

namespace medlink.Storage
{
    public class BodyDiagnosticStorage : BaseStorage<BodyDiagnosticInfo>, IBodyModelsStorage
    {
        private readonly HashSet<string> _bodySeries;
        
        public BodyDiagnosticStorage(ISerializer serializer, ISettings settings) : base(serializer, settings, settings.BodyDiagnosticFolder)
        {
            _bodySeries = new HashSet<string>(Directory
                .GetFiles(Folder, "*.*", SearchOption.AllDirectories)
                .Select(Path.GetFileName));
        }

        protected override void OnAdd(BodyDiagnosticInfo info, string filename)
        {
            _bodySeries.Add(info.ModelSeries + info.Revision);
        }

        public bool Contains(BodyDiagnosticInfo info)
        {
            return _bodySeries.Contains(info.ModelSeries);
        }
        
        public IEnumerable<string> GetAllModels()
        {
            return _bodySeries;
        }
    }
}