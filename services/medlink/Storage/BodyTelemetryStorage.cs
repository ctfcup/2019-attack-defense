using System.IO;
using medlink.Helpers;
using medlink.Storage.Models;

namespace medlink.Storage
{
    public interface IBodyTelemetryStorage : IBaseStorage<BodyTelemetry>
    {
    }

    public class BodyTelemetryStorage : BaseStorage<BodyTelemetry>, IBodyTelemetryStorage
    {
        public BodyTelemetryStorage(ISerializer serializer, ISettings settings) : base(serializer, settings)
        {
        }

        protected override string Folder => Settings.TelemetryFolder;
    }
}