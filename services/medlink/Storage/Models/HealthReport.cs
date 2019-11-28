using System.Collections.Generic;

namespace medlink.Storage.Models
{
    public class HealthReport
    {
        public Dictionary<string, string> CheckResults { get; set; }
        public string BodyOwner { get; set; }
    }
}