using System;
using System.IO;
using System.Threading.Tasks;
using log4net;
using medlink.Helpers;

namespace medlink.Storage
{
    public class FileDumper : IFileDumper
    {
        private readonly ILog _log;
        private readonly ISerializer _serializer;
        private readonly ISettings _settings;

        public FileDumper(ISettings settings, ISerializer serializer, ILog log)
        {
            _settings = settings;
            _log = log;
            _serializer = serializer;
        }

        public void Start(string filePath, Func<object> objectProvider)
        {
            Task.Run(async () =>
            {
                while (true)
                    try
                    {
                        File.WriteAllText(filePath, _serializer.Serialize(objectProvider()));
                        await Task.Delay(_settings.DumpInterval);
                    }
                    catch (Exception e)
                    {
                        _log.Error($"Can't dump {filePath} {Environment.NewLine} {e}");
                    }
            }).ConfigureAwait(false);
        }

        public bool TryFetch<T>(string filePath, out T result)
        {
            if (!File.Exists(filePath))
            {
                result = default;
                return false;
            }

            var source = File.ReadAllText(filePath);
            result = _serializer.Deserialize<T>(source);
            return true;
        }
    }
}