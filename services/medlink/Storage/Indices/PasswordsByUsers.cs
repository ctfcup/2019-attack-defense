using medlink.Helpers;
using medlink.Storage;

namespace medlink
{
    public class PasswordsByUsers : FileBasedIndex<string, string>, IPasswords
    {
        public PasswordsByUsers(ISettings settings, IFileDumper fileDumper) :
            base(fileDumper, settings.UsersFolder)
        {
        }
    }
}