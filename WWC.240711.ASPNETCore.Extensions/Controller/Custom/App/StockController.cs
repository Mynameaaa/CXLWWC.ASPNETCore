using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WWC._240711.ASPNETCore.Extensions
{
    /// <summary>
    /// 库存模块
    /// </summary>
    [CXLApiExplorerSettings("StockTag", nameof(CXLSwaggerGroup.Stock))]
    [CXLRoute("api/[controller]")]
    public class StockController : ICXLController
    {
        /// <summary>
        /// 获取库存
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string Get()
        {
            return "Get";
        }

        /// <summary>
        /// 提交库存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string Post(TestStockAddModel model)
        {
            return model.StockLocation + "---" + model.Size;
        }

        /// <summary>
        /// 删除库存
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public string Delete()
        {
            return "Delete";
        }

        /// <summary>
        /// 修复库存
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        public string Patch()
        {
            return "Patch";
        }

    }
}
