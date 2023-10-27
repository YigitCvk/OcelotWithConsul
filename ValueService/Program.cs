using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ValueService
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
                options.ValidateOnBuild = false;
                options.ValidateScopes = false;
            })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
