using System.Collections.Generic;
using System.Threading.Tasks;

namespace medlink.Storage
{
    public interface IBodyModelsStorage
    {
        IEnumerable<BodyIDDTO> GetAllModels();
        bool Contains(string series, string revision);
        bool Contains(string series);
        Task Add(BodyModelInfo info);
        Task<BodyModelInfo> Get(string series, string revision);
    }
}