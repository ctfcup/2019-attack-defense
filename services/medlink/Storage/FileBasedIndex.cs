using System;
using System.Collections.Concurrent;
using medlink.Storage;

namespace medlink
{
    public interface IFileBasedIndex<TValue, TKey>
    {
        void Add(TKey key, TValue value);
        TValue Get(TKey key);
        bool Contains(TKey key);
        bool TryGet(TKey key, out TValue result);
    }

    public abstract class FileBasedIndex<TValue, TKey> : IFileBasedIndex<TValue, TKey>
    {
        private readonly IFileDumper _fileDumper;
        protected ConcurrentDictionary<TKey, TValue> Index;

        protected FileBasedIndex(IFileDumper fileDumper, string folder)
        {
            _fileDumper = fileDumper;
            Initialize(folder);
        }

        public void Initialize(string filePath)
        {
            Index = _fileDumper.TryFetch<ConcurrentDictionary<TKey, TValue>>(filePath, out var indexSnapshot)
                ? indexSnapshot
                : new ConcurrentDictionary<TKey, TValue>();

            _fileDumper.Start(filePath, () => Index);
        }

        public void Add(TKey key, TValue value)
        {
            if (Index.ContainsKey(key))
                throw new ArgumentException($"Conflict. {key} already exist");
                
            Index[key] = value;
        }
        
        public TValue Get(TKey key)
        {
            if (!Index.ContainsKey(key))
                throw new ArgumentException($"Not found. {key} not found in index");

            return Index[key];
        }

        public bool Contains(TKey key)
        {
            return Index.ContainsKey(key);
        }

        public bool TryGet(TKey key, out TValue result)
        {
            return Index.TryGetValue(key, out result);
        }
    }
}