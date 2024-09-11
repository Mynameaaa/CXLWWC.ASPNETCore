using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Configuration.源码
{
    /// <summary>
    /// 表示基于文件的基类<see cref=“IConfigurationSource”/>。
    /// </summary>
    public abstract class CustomFileConfigurationSource : ICustomConfigurationSource
    {

        /// <summary>
        /// 用于访问文件的内容。
        /// </summary>
        public IFileProvider FileProvider { get; set; }

        /// <summary>
        /// 文件的路径。
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 确定是否可以选择加载文件。
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        /// 确定在基础文件更改时是否加载源。
        /// </summary>
        public bool ReloadOnChange { get; set; }

        /// <summary>
        /// 调用Load之前重新加载将等待的毫秒数。这有助于避免在文件完全写入之前触发重新加载。默认值为250。
        /// </summary>
        public int ReloadDelay { get; set; } = 250;

        /// <summary>
        /// 如果FileConfigurationProvider中发生未捕获的异常，将被调用。加载。
        /// </summary>
        public Action<FileLoadExceptionContext> OnLoadException { get; set; }

        /// <summary>
        /// 为此源构建<see cref=“ICustomConfigurationProvider”/>。
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>A <see cref="ICustomConfigurationProvider"/></returns>
        public abstract ICustomConfigurationProvider Build(ICustomConfigurationBuilder builder);

        /// <summary>
        /// 调用以使用构建器上的任何默认设置，如FileProvider或FileLoadExceptionHandler。
        /// </summary>
        /// <param name="builder">The <see cref="ICustomConfigurationBuilder"/>.</param>
        public void EnsureDefaults(ICustomConfigurationBuilder builder)
        {
            FileProvider = FileProvider;
            OnLoadException = OnLoadException;
        }

        /// <summary>
        /// 如果没有设置文件提供程序，对于绝对路径，这将创建一个物理文件提供程序查找最近的现有目录。
        /// </summary>
        public void ResolveFileProvider()
        {
            if (FileProvider == null &&
                !string.IsNullOrEmpty(Path) &&
                System.IO.Path.IsPathRooted(Path))
            {
                string directory = System.IO.Path.GetDirectoryName(Path);
                string pathToFile = System.IO.Path.GetFileName(Path);
                while (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    pathToFile = System.IO.Path.Combine(System.IO.Path.GetFileName(directory), pathToFile);
                    directory = System.IO.Path.GetDirectoryName(directory);
                }
                if (Directory.Exists(directory))
                {
                    FileProvider = new PhysicalFileProvider(directory);
                    Path = pathToFile;
                }
            }
        }

    }
}
