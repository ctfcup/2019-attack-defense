using medlink.Helpers;
using medlink.Storage;

namespace medlink
{
    public class Sessions : FileBasedIndex<string, string>, ISessions
    {
        public Sessions(ISettings settings, IFileDumper fileDumper) :
            base(fileDumper, settings.UserSessionsFolder)
        {
        }
       
    }
}