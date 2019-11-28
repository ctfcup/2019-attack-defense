using medlink.Storage;

namespace medlink
{
    public abstract class FileBasedIndex<TValue>
    {
        private readonly IFileDumper fileDumper;
        protected TValue Index;

        protected FileBasedIndex(IFileDumper fileDumper, TValue defaultValue, string folder)
        {
            this.fileDumper = fileDumper;
            Initialize(defaultValue, folder);
        }

        public void Initialize(TValue @default, string filePath)
        {
            Index = fileDumper.TryFetch<TValue>(filePath, out var usersSnapshot)
                ? usersSnapshot
                : @default;

            fileDumper.Start(filePath, () => Index);
        }
    }
}