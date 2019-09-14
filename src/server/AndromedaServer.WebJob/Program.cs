using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace AndromedaServer.WebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        public static async Task Main(string[] args)
        {
            // TODO:
            var builder = new HostBuilder()
                .UseEnvironment("Development")
                .ConfigureWebJobs(b =>
                {
                    //b.AddAzureStorageCoreServices()
                    //.AddAzureStorage()
                    //.AddServiceBus()
                    //.AddEventHubs();
                })
                .ConfigureAppConfiguration(b =>
                {
                    // Adding command line as a configuration source
                    //b.AddCommandLine(args);
                })
                .ConfigureLogging((context, b) =>
                {
                    b.SetMinimumLevel(LogLevel.Debug);
                    //b.AddConsole();

                    // If this key exists in any config, use it to enable App Insights
                    //string appInsightsKey = context.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
                    //if (!string.IsNullOrEmpty(appInsightsKey))
                    //{
                    //    b.AddApplicationInsightsWebJobs(o => o.InstrumentationKey = appInsightsKey);
                    //}
                })
                .ConfigureServices(services =>
                {
                    // add some sample services to demonstrate job class DI
                    //services.AddSingleton<ISampleServiceA, SampleServiceA>();
                    //services.AddSingleton<ISampleServiceB, SampleServiceB>();
                })
                ;//.UseConsoleLifetime();

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}
