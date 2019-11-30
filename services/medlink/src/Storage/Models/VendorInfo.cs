using System.Collections.Generic;

namespace medlink
{
    public class VendorInfo
    {
        public string Token { get; set; }
        public HashSet<string> ModelSeries { get; set; }
    }
}