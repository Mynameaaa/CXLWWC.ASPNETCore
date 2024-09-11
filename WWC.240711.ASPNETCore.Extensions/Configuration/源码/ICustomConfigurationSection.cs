using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Configuration.源码
{
    /// <summary>
    /// 表示应用程序配置值的一部分。
    /// </summary>
    public interface ICustomConfigurationSection : ICustomConfiguration
    {
        /// <summary>
        /// 获取此节在其父节中占用的键。
        /// </summary>
        string Key { get; }

        /// <summary>
        /// 获取此节的完整路径 <see cref="ICustomConfiguration"/>.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// 获取或设置节值。
        /// </summary>
        string? Value { get; set; }

    }
}
