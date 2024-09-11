using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;

namespace WWC._240711.ASPNETCore.Extensions.Configuration.源码
{
    /// <summary>
    /// 基于JSON文件 <see cref="FileConfigurationProvider"/>.
    /// </summary>
    public class CustomJsonConfigurationProvider : CustomFileConfigurationProvider, ICustomConfigurationProvider
    {
        /// <summary>
        /// 使用指定的源初始化新实例。
        /// </summary>
        /// <param name="source">源设置。</param>
        public CustomJsonConfigurationProvider(CustomJsonConfigurationSource source) : base(source) { }

        /// <summary>
        /// 从流中加载JSON数据。
        /// </summary>
        /// <param name="stream">阅读的溪流。</param>
        public override void Load(Stream stream)
        {
            try
            {
                //Data = JsonConfigurationFileParser.Parse(stream);
            }
            catch (JsonException e)
            {
                throw new FormatException("加载 Json 异常", e);
            }
        }
    }
}
