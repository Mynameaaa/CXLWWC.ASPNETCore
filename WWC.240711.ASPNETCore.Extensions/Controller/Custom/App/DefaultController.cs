using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WWC._240711.ASPNETCore.Extensions
{
    /// <summary>
    /// 默认模块
    /// </summary>
    [CXLRoute("api/[controller]")]
    public class DefaultController : ICXLController
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public DefaultController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        /// <summary>
        /// 获取默认
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string Get()
        {
            var ctx = _contextAccessor.HttpContext;
            return "Get";
        }

        /// <summary>
        /// 提交默认
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string Post(TestStockAddModel model)
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
