using medlink.Helpers;
using medlink.Storage;

namespace medlink
{
    public class Users : FileBasedIndex<string, string>, IUsers
    {
        public Users(ISettings settings, IFileDumper fileDumper) :
            base(fileDumper, settings.UsersFolder)
        {
        }
    }
}