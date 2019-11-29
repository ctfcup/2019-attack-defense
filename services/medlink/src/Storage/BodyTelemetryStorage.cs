using medlink.Helpers;
using medlink.Storage.Models;

namespace medlink.Storage
{
    public class BodyTelemetryStorage : BaseStorage<BodyTelemetry>, IBodyTelemetryStorage
    {
        public BodyTelemetryStorage(ISerializer serializer, ISettings settings) : base(serializer, settings, settings.TelemetryFolder)
        {
        }

    }
}