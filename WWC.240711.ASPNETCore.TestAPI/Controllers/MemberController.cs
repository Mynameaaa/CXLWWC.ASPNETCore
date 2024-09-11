using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WWC._240711.ASPNETCore.TestAPI.Controllers
{
    /// <summary>
    /// 会员模块
    /// </summary>
    [CXLApiExplorerSettings("UserTag", nameof(CXLSwaggerGroup.User))]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MemberController : ControllerBase
    {
        /// <summary>
        /// 获取会员
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionApiExplorerSettings(true)]
        public string MemberGet()
        {
            return "Get";
        }

        /// <summary>
        /// 提交会员
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string MemberPost()
        {
            return "Post";
        }

        /// <summary>
        /// 删除会员
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public string MemberDelete()
        {
            return "Delete";
        }

        /// <summary>
        /// 修复会员
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        [CXLApiExplorerSettingsAttribute("Back", nameof(CXLSwaggerGroup.Back))]
        public string Patch()
        {
            return "Patch";
        }

    }
}
