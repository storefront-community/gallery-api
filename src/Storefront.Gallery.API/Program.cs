using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Storefront.Gallery.API
{
    [ExcludeFromCodeCoverage]
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingConext, config) =>
                {
                    config.AddEnvironmentVariables(prefix: "StorefrontCommunity_Gallery_");
                })
                .UseStartup<Startup>()
                .UseSentry();
    }
}
