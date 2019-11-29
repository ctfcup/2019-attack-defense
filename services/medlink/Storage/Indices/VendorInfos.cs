using System.Collections.Generic;
using System.Security.Policy;
using log4net;
using medlink.Helpers;
using medlink.Storage;

namespace medlink
{
    public class VendorInfo
    {
        public string Token { get; set; }
        public HashSet<string> ModelSeries { get; set; }
    }


    public class VendorInfos : FileBasedIndex<VendorInfo, string>, IVendorInfos
    {
        private readonly ILog _log;
        private readonly Dictionary<string, VendorInfo> tokens;

        public VendorInfos(ISettings settings, IFileDumper fileDumper) : base(fileDumper, settings.VendorsFolder)
        {
        }

    }
}