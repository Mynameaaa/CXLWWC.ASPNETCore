using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.FileServer.Custom
{
    public static class CXLStaticFileExtensions
    {

        /// <summary>
        /// 启用静态文件访问
        /// </summary>
        /// <param name="app"></param>
        /// <param name="directoryName"></param>
        /// <param name="requestPath"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCXLStaticFile(this WebApplication app, string directoryName, string requestPath = "")
        {
            string rootDirectory = Path.Combine(app.Environment.ContentRootPath, directoryName);

            if (!Directory.Exists(rootDirectory))
                Directory.CreateDirectory(rootDirectory);

            return app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(rootDirectory),
                RequestPath = requestPath
            });
        }

        /// <summary>
        /// 启用文件夹访问
        /// </summary>
        /// <param name="app"></param>
        /// <param name="directoryName"></param>
        /// <param name="requestPath"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCXLDirectoryBrowser(this WebApplication app, string directoryName, string requestPath = "")
        {
            string rootDirectory = Path.Combine(app.Environment.ContentRootPath, directoryName);

            if (!Directory.Exists(rootDirectory))
                Directory.CreateDirectory(rootDirectory);

            return app.UseDirectoryBrowser(new DirectoryBrowserOptions()
            {
                FileProvider = new PhysicalFileProvider(rootDirectory),
                RequestPath = requestPath
            });
        }

        /// <summary>
        /// 启用返回默认文件
        /// </summary>
        /// <param name="app"></param>
        /// <param name="directoryName"></param>
        /// <param name="requestPath"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCXLDefaultFiles(this WebApplication app, string directoryName, string requestPath = "", params string[] files)
        {
            string rootDirectory = Path.Combine(app.Environment.ContentRootPath, directoryName);

            if (!Directory.Exists(rootDirectory))
                Directory.CreateDirectory(rootDirectory);

            return app.UseDefaultFiles(new DefaultFilesOptions()
            {
                DefaultFileNames = files ?? ["default.html", "index.html"],
                RequestPath = requestPath,
            });
        }

    }
}
