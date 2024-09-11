using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Configuration.源码
{
    /// <summary>
    /// 将JSON文件表示为<see cref=“IConfigurationSource”/>。
    /// </summary>
    public class CustomJsonConfigurationSource : CustomFileConfigurationSource, ICustomConfigurationSource
    {
        /// <summary>
        /// 为此源构建<see cref=“CustomJsonConfigurationProvider”/>。
        /// </summary>
        /// <param name="builder">The <see cref="ICustomConfigurationBuilder"/>.</param>
        /// <returns>A <see cref="CustomJsonConfigurationProvider"/></returns>
        public override ICustomConfigurationProvider Build(ICustomConfigurationBuilder builder)
        {
            return new CustomJsonConfigurationProvider(this);
        }
    }
}
