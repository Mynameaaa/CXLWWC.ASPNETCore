using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WWC._240711.ASPNETCore.TestAPI.Controllers
{
    /// <summary>
    /// 默认模块
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        /// <summary>
        /// 获取默认
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string Get()
        {
            return "Get";
        }

        /// <summary>
        /// 提交默认
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string Post(StockAddModel model)
        {
            return model.StockLocation + "---" + model.Size;
        }

        /// <summary>
        /// 删除默认
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public string Delete()
        {
            return "Delete";
        }

        /// <summary>
        /// 修复默认
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        public string Patch()
        {
            return "Patch";
        }

    }
}
