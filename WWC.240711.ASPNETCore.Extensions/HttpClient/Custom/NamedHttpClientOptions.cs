using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions
{
    public class NamedHttpClientOptions
    {

        /// <summary>
        /// 根路径
        /// </summary>
        [Required(ErrorMessage = "客户端名称不能为空")]
        public string Name { get; set; }

        /// <summary>
        /// 根路径
        /// </summary>
        [Required(ErrorMessage = "根路径不能为空")]
        [Url(ErrorMessage = "BaseUrl must be a valid URL.")]
        public string BaseUrl { get; set; }

        /// <summary>
        /// 默认请求头
        /// </summary>
        public Dictionary<string,string> DefaultHeaders { get; set; }

    }
}
