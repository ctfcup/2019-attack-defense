using System.Security.Cryptography;
using System.Text;

namespace medlink.Helpers
{
    public class Utils
    {
        public static string GetSha256(string source)
        {
            using (var sha256Hash = SHA256.Create())
            {
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(source));

                return Encoding.Default.GetString(bytes);
            }
        }
        
        public static byte[] GetSha256Bytes(string source)
        {
            using (var sha256Hash = SHA256.Create())
            {
                return sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(source));
            }
        }
    }
}