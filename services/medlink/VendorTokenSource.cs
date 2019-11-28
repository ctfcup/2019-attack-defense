using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using log4net;
using medlink.Helpers;
using medlink.Storage;

namespace medlink
{
    public interface IVendorTokenSource
    {
        bool SetToken(string login, string vendorToken);
        bool CheckToken(string login, string vendorToken);
    }

    public class VendorTokenSource : IVendorTokenSource
    {
        private readonly ILog _log;
        private readonly Dictionary<string, string> tokens;

        public VendorTokenSource(ISettings settings, IFileDumper fileDumper)
        {
            tokens = fileDumper.TryFetch<Dictionary<string, string>>(settings.VendorTokensFolder, out var usersSnapshot)
                ? usersSnapshot
                : new Dictionary<string, string>();

            fileDumper.Start(settings.VendorTokensFolder, () => tokens);
        }

        public bool SetToken(string login, string vendorToken)
        {
            if (!tokens.ContainsKey(login))
            {
                tokens[login] = GetHash(vendorToken);
                return true;
            }

            return false;
        }
        
        public bool CheckToken(string login, string vendorToken) => tokens.ContainsKey(login) && tokens[login].Equals(GetHash(vendorToken));

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