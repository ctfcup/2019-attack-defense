using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using log4net;
using medlink.Helpers;
using medlink.Storage;

namespace medlink
{
    public class AuthProvider : FileBasedIndex<Dictionary<string,string>>, IAuthProvider
    {
        private readonly ISettings _settings;
        private readonly ILog _log;

        public AuthProvider(ISettings settings, IFileDumper fileDumper) :
            base(fileDumper, new Dictionary<string, string>(), settings.UsersFolder)
        {
            _settings = settings;
        }

        public bool AddUserOrCheckPass(string login, string pass)
        {
            if (!Index.ContainsKey(login))
            {
                Index[login] = GetHash(pass);
                return true;
            }

            return Index[login].Equals(GetHash(pass));
        }

        private string GetHash(string source)
        {
            using (var sha256Hash = SHA256.Create())
            {
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(source));

                return Encoding.Default.GetString(bytes);
            }
        }
    }
}