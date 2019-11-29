using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using medlink.Helpers;
using medlink.Storage;

namespace medlink
{
    public abstract class FileBasedIndex<TValue, TKey> : IFileBasedIndex<TValue, TKey>
    {
        private readonly IFileDumper _fileDumper;
        private ConcurrentDictionary<TKey, Record<TValue>> _index;
        private IReadOnlyDictionary<TKey, Record<TValue>> Index => _index;

        protected FileBasedIndex(IFileDumper fileDumper, ISettings settings, string folder)
        {
            _fileDumper = fileDumper;
            Initialize(folder);
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(settings.Ttl);
                    try
                    {
                        var expired = _index.Where(pair => DateTime.UtcNow - pair.Value.Timestamp > settings.Ttl)
                            .Select(pair => pair.Key).ToList();

                        foreach (var key in expired)
                        {
                            _index.TryRemove(key, out _);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            });
        }

        public void Initialize(string filePath)
        {
            _index = _fileDumper.TryFetch<ConcurrentDictionary<TKey, Record<TValue>>>(filePath, out var indexSnapshot)
                ? indexSnapshot
                : new ConcurrentDictionary<TKey, Record<TValue>>();

            _fileDumper.Start(filePath, () => Index);
        }

        public void Add(TKey key, TValue value)
        {
            if (Index.ContainsKey(key))
                throw new ArgumentException($"Conflict. {key} already exist");
                
            _index[key] = new Record<TValue>
            {
                Timestamp = DateTime.UtcNow,
                Value = value
            };
        }
        
        public TValue Get(TKey key)
        {
            if (!Index.ContainsKey(key))
                throw new ArgumentException($"Not found. {key} not found in index");

            return _index[key].Value;
        }

        public bool Contains(TKey key)
        {
            return Index.ContainsKey(key);
        }

        public bool TryGet(TKey key, out TValue result)
        {
            var getResult = Index.TryGetValue(key, out var value);
            result = value.Value;
            return getResult;
        }

        public int Count => Index.Count;

        public IEnumerator<KeyValuePair<TKey, Record<TValue>>> GetEnumerator()
        {
            return _index.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    
    public class Record<TValue>
    {
        public DateTime Timestamp { get; set; }
        public TValue Value { get; set; }
    }
}