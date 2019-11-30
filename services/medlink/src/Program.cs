using System.IO;
using System.Net;
using System.Threading;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace medlink
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ThreadPool.SetMaxThreads(128, 128);
            ThreadPool.SetMinThreads(128, 128);
            
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseKestrel(options =>
                        {
#if DEBUG
                            options.Listen(IPAddress.Loopback, 5002);
#endif
                        })
                        .UseStartup<Startup>();
                });
        }
    }
}