using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Configuration.源码
{
    /// <summary>
    /// 表示一组键/值应用程序配置属性。
    /// </summary>
    public interface ICustomConfiguration
    {

        //获取或设置配置值。
        string? this[string key] { get; set; }

        /// <summary>
        /// 获取具有指定键的配置子节。
        /// </summary>
        /// <remarks>
        /// 此方法永远不会返回<c>null</c>。如果没有找到与指定密钥匹配的子部分，
        /// 将返回一个空的<see-cref=“IConfigurationSection”/>。
        /// </remarks>
        ICustomConfigurationSection GetSection(string key);

        /// <summary>
        /// 获取直接子代配置子节。
        /// </summary>
        /// <returns>将整个配置树结构转成一个集合</returns>
        IEnumerable<ICustomConfigurationSection> GetChildren();

        /// <summary>
        /// 返回一个<see cref=“IChangeToken”/>，可用于观察此配置何时重新加载。
        /// </summary>
        /// <returns>A <see cref="IChangeToken"/>.</returns>
        IChangeToken GetReloadToken();
    }
}
