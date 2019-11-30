using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using medlink.Helpers;
using medlink.Storage;

namespace medlink
{
    public abstract class FileBasedIndex<TValue, TKey> : IFileBasedIndex<TValue, TKey>
    {
        private class IndexDTO
        {
            public ConcurrentDictionary<TKey, Record<TValue>> Index { get; set; }
            public int Id { get; set; }
        }
        
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

        private void Initialize(string filePath)
        {
            if (_fileDumper.TryFetch<IndexDTO>(filePath, out var dto))
            {
                _index = dto.Index;
                _id = dto.Id;
            }
            else
            {
                _index = new ConcurrentDictionary<TKey, Record<TValue>>();
            }
            
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
            Interlocked.Increment(ref _id);
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
            result = getResult ? value.Value : default;
            return getResult;
        }

        public int Count => Index.Count;
        public int Id => _id;
        private int _id;

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
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public TValue Value { get; set; }
    }
}