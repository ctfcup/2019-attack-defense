using System.Collections.Generic;

namespace medlink.Storage
{
    public interface IBodyModelsStorage : IBaseStorage<BodyDiagnosticInfo>
    {
        IEnumerable<SeriesIndexRecord> GetAllModels();
        bool Contains(string filename);
    }
}