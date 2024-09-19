using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Http.Custom
{
    public class NamedHttpClientOptions
    {

        /// <summary>
        /// 根路径
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 根路径
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// 默认请求头
        /// </summary>
        public Dictionary<string,string> DefaultHeaders { get; set; }

    }
}
