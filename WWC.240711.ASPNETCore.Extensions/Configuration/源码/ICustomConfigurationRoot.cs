using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Configuration.源码
{
    /// <summary>
    /// 表示<see cref=“ICustomConfiguration”/>层次结构的根。
    /// </summary>
    public interface ICustomConfigurationRoot : ICustomConfiguration
    {
        /// <summary>
        /// 强制从底层<see cref=“ICustomConfigurationProvider”/>重新加载配置值。
        /// </summary>
        void Reload();

        /// <summary>
        /// 此配置的<see cref=“ICustomConfigurationProvider”/>。
        /// </summary>
        IEnumerable<ICustomConfigurationProvider> Providers { get; }
    }
}
