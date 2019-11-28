using System;
using System.IO;
using System.Threading.Tasks;
using medlink.Helpers;

namespace medlink.Storage
{
    public abstract class BaseStorage<TValue> : IBaseStorage<TValue>
    {
        private readonly ISerializer _serializer;
        protected readonly ISettings Settings;

        public BaseStorage(ISerializer serializer, ISettings settings)
        {
            _serializer = serializer;
            Settings = settings;
            StartCleanup().ConfigureAwait(false);
        }

        protected abstract string Folder { get; }

        public async Task StartCleanup()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
                string[] allfiles = Directory.GetFiles(Folder, "*", SearchOption.AllDirectories);
                foreach (var file in allfiles)
                {
                    if (DateTime.UtcNow - File.GetCreationTime(file) >= Settings.Ttl)
                    {
                        File.Delete(file);
                    }
                }
            }
        }
        
        public async Task Add(TValue info, string filename)
        {
            var path = GetPath(filename);
            var folder = Path.GetDirectoryName(path);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            
            await File.WriteAllTextAsync(path, _serializer.Serialize(info));
        }

        public async Task<TValue> Get(string filename)
        {
            var record = await File.ReadAllTextAsync(GetPath(filename));
            return _serializer.Deserialize<TValue>(record);
        }

        public bool Exist(string filename)
        {
            return File.Exists(GetPath(filename));
        }

        private string GetPath(string name)
        {
            if (name.Contains(".."))
                throw new ArgumentException("File path should not contain \"..\" :)");

            var path = Path.Combine(Folder, name);
            return path;
        }
    }
}