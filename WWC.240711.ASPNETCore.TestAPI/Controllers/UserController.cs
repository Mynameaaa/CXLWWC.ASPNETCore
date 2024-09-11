using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WWC._240711.ASPNETCore.TestAPI.Controllers
{
    /// <summary>
    /// 用户模块
    /// </summary>
    [CXLApiExplorerSettings("UserTag", nameof(CXLSwaggerGroup.User))]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string Get()
        {
            return "Get";
        }

        /// <summary>
        /// 提交用户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string Post()
        {
            return "Post";
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public string Delete()
        {
            return "Delete";
        }

        /// <summary>
        /// 修复用户
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        public string Patch()
        {
            return "Patch";
        }
    }
}
