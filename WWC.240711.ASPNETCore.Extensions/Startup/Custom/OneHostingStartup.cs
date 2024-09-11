using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: HostingStartup(typeof(WWC._240711.ASPNETCore.Extensions.Startup.使用.OneHostingStartup))]
//记得在 launchSettings.json 中添加 "ASPNETCORE_HOSTINGSTARTUPASSEMBLIES": "命名空间"
namespace WWC._240711.ASPNETCore.Extensions.Startup.使用
{
    //比 Startup 先执行
    internal class OneHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((config) =>
            {
                Console.WriteLine("ConfigureAppConfiguration");
            });

            builder.ConfigureServices(services =>
            {

                Console.WriteLine("ConfigureServices");
            });

            ////如果存在 StartupFilter 则会被 IHostingStartup 覆盖，两者冲突
            //builder.Configure(app =>
            //{
            //    Console.WriteLine("Configure");
            //});
        }
    }
}
