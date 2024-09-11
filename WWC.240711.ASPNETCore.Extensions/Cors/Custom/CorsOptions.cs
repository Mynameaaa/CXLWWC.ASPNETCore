using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions
{
    public class CorsOptions
    {
        /// <summary>
        /// 策略名称
        /// </summary>
        public string PolicyName { get; set; }

        /// <summary>
        /// 是否允许所有网站
        /// </summary>
        public bool AllowAnyOrigins { get; set; }

        /// <summary>
        /// 允许网站
        /// </summary>
        public string[] WithOrigins { get; set; }

        /// <summary>
        /// 是否允许所有网站
        /// </summary>
        public bool AllowAnyMethods { get; set; }

        /// <summary>
        /// 允许方法
        /// </summary>
        public string[] WithMethods { get; set; }

        /// <summary>
        /// 允许所有
        /// </summary>
        public bool AllowAnyHeaders { get; set; }

        /// <summary>
        /// 允许的自定义头部
        /// </summary>
        public string[] WithHeaders { get; set; }

    }
}
