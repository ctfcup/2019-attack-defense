using System.Collections.Generic;

namespace medlink.Storage.Models
{
    public class HealthReport
    {
        public HealthReport()
        {
            CheckResults = new Dictionary<string, string>();
        }

        public Dictionary<string, string> CheckResults { get; set; }
        public string Error { get; set; }
    }
}