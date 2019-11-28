using System.Collections.Generic;

namespace medlink.Storage
{
    public interface IBodyModelsStorage : IBaseStorage<BodyDiagnosticInfo>
    {
        IEnumerable<string> GetAllModels();
        bool Contains(BodyDiagnosticInfo info);
    }
}