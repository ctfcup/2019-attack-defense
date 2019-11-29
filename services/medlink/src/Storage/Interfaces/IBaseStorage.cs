using System.Threading.Tasks;

namespace medlink.Storage
{
    public interface IBaseStorage<TValue>
    {
        Task Add(TValue info, string filename);
        Task<TValue> Get(string filename);
        bool Exist(string filename);
    }
}