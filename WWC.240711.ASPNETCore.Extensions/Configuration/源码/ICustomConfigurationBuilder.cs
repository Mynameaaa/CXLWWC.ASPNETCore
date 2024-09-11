using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Configuration.源码
{
    public interface ICustomConfigurationBuilder
    {
        /// <summary>
        /// 获取可用于在<see-cref="IConfigurationBuilder"/>之间共享数据的键/值集合 已注册
        /// <see cref="IConfigurationSource"/> 
        /// </summary>
        IDictionary<string, object> Properties { get; }

        /// <summary>
        /// 获取用于获取配置值的源
        /// </summary>
        IList<ICustomConfigurationSource> Sources { get; }

        /// <summary>
        /// 添加新的配置源。
        /// </summary>
        /// <param name="source">要添加的配置源。</param>
        /// <returns>The same <see cref="ICustomConfigurationBuilder"/>.</returns>
        ICustomConfigurationBuilder Add(ICustomConfigurationSource source);

        /// <summary>
        /// 使用在中注册的源集合中的键和值构建一个<see-cref=“IConfiguration”/>
        /// <see cref="Sources"/>.
        /// </summary>
        /// <returns>An <see cref="IConfigurationRoot"/>使用来自注册源的键和值。.</returns>
        ICustomConfigurationRoot Build();
    }
}
