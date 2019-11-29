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
        protected readonly string Folder;

        public BaseStorage(ISerializer serializer, ISettings settings, string folder)
        {
            _serializer = serializer;
            Settings = settings;
            Folder = folder;
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            
            StartCleanup().ConfigureAwait(false);
        }


        public async Task Add(TValue info, string filename)
        {
            var path = GetPath(filename);
            var folder = Path.GetDirectoryName(path);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            await File.WriteAllTextAsync(path, _serializer.Serialize(info));
            OnAdd(info, filename);
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

        protected virtual void OnAdd(TValue info, string filename)
        {
        }

        private async Task StartCleanup()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
                var allfiles = Directory.GetFiles(Folder, "*", SearchOption.AllDirectories);
                foreach (var file in allfiles)
                    if (DateTime.UtcNow - File.GetCreationTime(file) >= Settings.Ttl)
                        File.Delete(file);
            }
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