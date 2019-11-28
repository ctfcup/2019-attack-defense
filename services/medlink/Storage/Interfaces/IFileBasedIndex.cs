namespace medlink
{
    public interface IFileBasedIndex<TValue, TKey>
    {
        void Add(TKey key, TValue value);
        TValue Get(TKey key);
        bool Contains(TKey key);
        bool TryGet(TKey key, out TValue result);
        int Count { get; }
    }
}