using System.Collections.Generic;

namespace medlink.Storage.Models
{
    public class BodyTelemetry
    {
        public string BodyModelSeries { get; set; }
        public string BodyRevision { get; set; }
        public string BodyID { get; set; }
        public Dictionary<string, double> HardwareTelemetry { get; set; }
    }
}