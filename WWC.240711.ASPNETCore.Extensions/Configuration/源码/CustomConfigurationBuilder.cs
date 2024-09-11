using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Configuration.源码
{
    /// <summary>
    /// 用于构建基于键/值的配置设置，以在应用程序中使用。
    /// </summary>
    public class CustomConfigurationBuilder : ICustomConfigurationBuilder
    {
        /// <summary>
        /// 返回用于获取配置值的源。
        /// </summary>
        public IList<ICustomConfigurationSource> Sources { get; } = new List<ICustomConfigurationSource>();

        /// <summary>
        /// 获取可用于在<see-cref="IConfigurationBuilder"/>之间共享数据的键/值集合 已注册
        /// <see cref="IConfigurationSource"/> 
        /// </summary>
        /// </summary>
        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        /// <summary>
        /// 添加新的配置源。
        /// </summary>
        /// <param name="source">要添加的配置源。</param>
        /// <returns>The same <see cref="ICustomConfigurationBuilder"/>.</returns>
        public ICustomConfigurationBuilder Add(ICustomConfigurationSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            Sources.Add(source);
            return this;
        }

        /// <summary>
        /// 使用在中注册的源集合中的键和值构建一个<see-cref=“IConfiguration”/>
        /// <see cref="Sources"/>.
        /// </summary>
        /// <returns>An <see cref="IConfigurationRoot"/>使用来自注册源的键和值。.</returns>
        public ICustomConfigurationRoot Build()
        {
            var providers = new List<ICustomConfigurationProvider>();
            foreach (ICustomConfigurationSource source in Sources)
            {
                ICustomConfigurationProvider provider = source.Build(this);
                providers.Add(provider);
            }
            return new CustomConfigurationRoot(providers);
        }
    }
}
