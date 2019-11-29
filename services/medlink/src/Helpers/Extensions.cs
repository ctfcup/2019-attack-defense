using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace medlink.Helpers
{
    public static class Extensions
    {
        public static async Task<string> ReadContentAsync(this HttpListenerRequest request)
        {
            return await request.InputStream.ReadToEndAsync();
        }

        public static async Task Send(this HttpListenerResponse response, int statusCode, string message = "")
        {
            response.StatusCode = statusCode;
            await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(message));
        }

        public static async Task<string> ReadToEndAsync(this Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}