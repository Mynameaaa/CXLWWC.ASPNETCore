using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WWC._240711.ASPNETCore.TestAPI.Controllers
{
    /// <summary>
    /// 汤姆模块
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TomController : ControllerBase
    {
        /// <summary>
        ///获取汤姆
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string Get()
        {
            return "Get";
        }

        /// <summary>
        /// 提交汤姆
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string Post()
        {
            return "Post";
        }

        /// <summary>
        /// 删除汤姆
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public string Delete()
        {
            return "Delete";
        }

        /// <summary>
        /// 修复汤姆
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        public string Patch()
        {
            return "Patch";
        }
    }
}
