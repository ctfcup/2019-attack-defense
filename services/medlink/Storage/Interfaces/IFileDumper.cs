using System;

namespace medlink.Storage
{
    public interface IFileDumper
    {
        void Start(string filePath, Func<object> objectProvider);
        bool TryFetch<T>(string filePath, out T result);
    }
}