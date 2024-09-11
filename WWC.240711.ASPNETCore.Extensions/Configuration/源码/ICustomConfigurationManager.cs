using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Configuration.源码
{
    /// <summary>
    /// 表示可变配置对象。.
    /// </summary>
    /// <remarks>
    /// 它既是一个<see cref=“ICustomConfigurationBuilder”/>，也是一个<see cref=“ICustomConfiguration”/>。
    /// </remarks>
    public interface ICustomConfigurationManager : ICustomConfiguration, ICustomConfigurationBuilder
    {

    }
}
