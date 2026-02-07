using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TelemetryGenerator
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
          .ConfigureServices((hostContext, services) =>
          {
            // Register hosted telemetry publisher (no EF/DbContext)
            services.AddHostedService<TelemetryPublisher>();
          });
  }
}
