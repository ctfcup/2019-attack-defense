using System.Security.Policy;
using medlink.Helpers;
using medlink.Storage;

namespace medlink
{
    public class VendorInfos : FileBasedIndex<VendorInfo, string>, IVendorInfos
    {
        public VendorInfos(ISettings settings, IFileDumper fileDumper) : base(fileDumper, settings, settings.VendorsFolder)
        {
        }

    }
}