using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Extensions.ApplicationBuilder.Custom.App;
using WWC._240711.ASPNETCore.Extensions.Server.Custom;

namespace WWC._240711.ASPNETCore.Extensions.WebHost.Custom
{
    public class CustomWebHostBuilder
    {
        public IHostBuilder HostBuilder { get; }
        public ICustomApplicationBuilder ApplicationBuilder { get; }
        public CustomWebHostBuilder(IHostBuilder hostBuilder, ICustomApplicationBuilder applicationBuilder)
        {
            HostBuilder = hostBuilder;
            ApplicationBuilder = applicationBuilder;
        }
    }
    public static partial class Extensions
    {
        public static CustomWebHostBuilder UseHttpListenerServer(this CustomWebHostBuilder builder, params string[] urls)
        {
            builder.HostBuilder.ConfigureServices((context, svcs) => svcs.AddSingleton<ICustomServer>(new CustomHttpListenerServer(urls)));
            return builder;
        }

        public static CustomWebHostBuilder Configure(this CustomWebHostBuilder builder, Action<ICustomApplicationBuilder> configure)
        {
            configure?.Invoke(builder.ApplicationBuilder);
            return builder;
        }
    }
    public static partial class Extensions
    {
        public static IHostBuilder ConfigureWebHost(this IHostBuilder builder, Action<CustomWebHostBuilder> configure)
        {
            var webHostBuilder = new CustomWebHostBuilder(builder, new CustomApplicationBuilder());
            configure?.Invoke(webHostBuilder);
            builder.ConfigureServices((context, svcs) => svcs.AddSingleton<IHostedService>(provider =>
            {
                var server = provider.GetRequiredService<ICustomServer>();
                var handler = webHostBuilder.ApplicationBuilder.Build();
                return new CustomWebHostedService(server, handler);
            }));
            return builder;
        }
    }
}
