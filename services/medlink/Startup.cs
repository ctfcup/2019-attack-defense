using Autofac;
using Autofac.Extensions.DependencyInjection;
using log4net;
using medlink.HealthChecks;
using medlink.Helpers;
using medlink.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace medlink
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public ILifetimeScope AutofacContainer { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });
            services.AddOptions();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterInstance(new Settings()).As<ISettings>();
            builder.RegisterType<Serializer>().As<ISerializer>();
            builder.RegisterInstance(LogManager.GetLogger(GetType())).As<ILog>();
            builder.RegisterType<SessionSource>().As<ISessionSource>();
            ReisterStorages(builder);
        }

        private static void ReisterStorages(ContainerBuilder builder)
        {
            builder.RegisterType<Sessions>().As<ISessions>().SingleInstance();
            builder.RegisterType<PasswordsByUsers>().As<IPasswords>().SingleInstance();
            builder.RegisterType<FileDumper>().As<IFileDumper>().SingleInstance();
            builder.RegisterType<BodyTelemetryStorage>().As<IBodyTelemetryStorage>().SingleInstance();
            builder.RegisterType<HealthChecker>().As<IHealthChecker>().SingleInstance();
            builder.RegisterType<BodyDiagnosticStorage>().As<IBodyModelsStorage>().SingleInstance();
            builder.RegisterType<VendorInfos>().As<IVendorInfos>().SingleInstance();
            builder.RegisterType<SeriesIndex>().As<ISeriesIndex>().SingleInstance();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            AutofacContainer = app.ApplicationServices.GetAutofacRoot();

#if DEBUG
            app.UseDeveloperExceptionPage();
#endif
#if RELEASE
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
#endif

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

#if DEBUG
                spa.UseReactDevelopmentServer("start");
#endif
            });
        }
    }
}