using System.Collections.Generic;
using log4net;
using medlink.Helpers;
using medlink.Storage;

namespace medlink
{
    public class VendorTokens : FileBasedIndex<string, string>, IVendorTokens
    {
        private readonly ILog _log;
        private readonly Dictionary<string, string> tokens;

        public VendorTokens(ISettings settings, IFileDumper fileDumper) : base(fileDumper, settings.VendorTokensFolder)
        {
        }

    }
}