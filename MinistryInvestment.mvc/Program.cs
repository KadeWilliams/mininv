using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MinistryInvestment.Mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseDefaultServiceProvider((context, options) =>
                {
                    if (context.HostingEnvironment.IsDevelopment() || context.HostingEnvironment.IsEnvironment("Local"))
                    {
                        options.ValidateScopes = true;
                        options.ValidateOnBuild = true;
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}