using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.Extensions.ThirdParty.Models
{
    /// <summary>
    /// Redis连接配置
    /// </summary>
    public class RedisConnectOptions
    {

        /// <summary>
        /// Consul 服务
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Consul 端口
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// 连接密码
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// 连接超时时间
        /// </summary>
        public int? ConnectTimeout { get; set; }

        /// <summary>
        /// 读取超时时间
        /// </summary>
        public int? ReadTimeout { get; set; }

        /// <summary>
        /// 写入超时时间
        /// </summary>
        public int? WriteTimeout { get; set; }

        /// <summary>
        /// 连接最大闲置时间
        /// </summary>
        public int? MaxIdleTime { get; set; }

        /// <summary>
        /// 连接池大小
        /// </summary>
        public int? PoolSize { get; set; }

    }
}
