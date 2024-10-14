using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WWC._240711.ASPNETCore.Auth;
using WWC._240711.ASPNETCore.AuthPlatform.Model;
using WWC._240711.ASPNETCore.AuthPlatform.Servcies;

namespace WWC._270711.ASPNETCore.AuthPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private readonly IDownLoadFileService _downLoadFileService;
        private readonly ITokenService _tokenService;

        public OAuthController(IDownLoadFileService downLoadFileService
            , ITokenService tokenService)
        {
            _downLoadFileService = downLoadFileService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// 获取 Token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        public async Task<TokenModel> Login(ValidationUserModel model)
        {
            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
            {
                throw new Exception("用户名或密码不能为空");
            }

            var keys = await _downLoadFileService.GetTokenKeyFile();
            var privateKey = keys[0];
            var claims = new Dictionary<string, string>()
            {
                { "userName", model.UserName },
                { "userId", "666" },
                { "userPower", "Admin" },
                { "scope", "User,Back,Stock,Order" },
            };

            var tokenModel = _tokenService.GenerateJwtToken(privateKey, claims);

            return tokenModel;
        }

    }
}
