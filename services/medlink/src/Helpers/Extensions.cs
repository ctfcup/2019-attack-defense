using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace medlink.Helpers
{
    public static class Extensions
    {
        public static async Task<string> ReadToEndAsync(this Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}