using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace AtomicCore.IOStorage.StoragePort
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel((context, options) =>
                    {
                        //设置应用服务器Kestrel请求体最大为50MB
                        options.Limits.MaxRequestBodySize = 52428800;
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
