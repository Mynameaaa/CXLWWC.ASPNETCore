using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WWC._240711.ASPNETCore.Auth;
using WWC._240711.ASPNETCore.AuthPlatform.Cache;
using WWC._240711.ASPNETCore.AuthPlatform.Model;
using WWC._240711.ASPNETCore.AuthPlatform.Servcies;

namespace WWC._270711.ASPNETCore.AuthPlatform.Controllers
{
    [Route("api/auth/[controller]")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private readonly IDownLoadFileService _downLoadFileService;
        private readonly ITokenService _tokenService;
        private readonly IFileCacheService _fileCacheService;

        public OAuthController(IDownLoadFileService downLoadFileService
            , ITokenService tokenService
            , IFileCacheService fileCacheService)
        {
            _downLoadFileService = downLoadFileService;
            _tokenService = tokenService;
            _fileCacheService = fileCacheService;
        }

        /// <summary>
        /// 获取 Token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost("login")]
        public async Task<TokenModel> Login(ValidationUserModel model)
        {
            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
            {
                throw new Exception("用户名或密码不能为空");
            }

            byte[]? privateKey;
            byte[]? publicKey;
            if (_fileCacheService.HasKey(CacheConstantKeys.TokenPrivateKey))
            {
                privateKey = _fileCacheService.GetFile(CacheConstantKeys.TokenPrivateKey);
            }
            else
            {
                var keys = await _downLoadFileService.GetTokenKeyFile();
                privateKey = keys[0];
                publicKey = keys[1];
                _fileCacheService.CacheFile(CacheConstantKeys.TokenPrivateKey, privateKey);
                _fileCacheService.CacheFile(CacheConstantKeys.TokenPublicKey, publicKey);
            }

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

        /// <summary>
        /// 获取私钥文件
        /// </summary>
        /// <returns></returns>
        [HttpGet("privateKey")]
        public async Task<IActionResult> GetPrivateKeyFile()
        {
            byte[]? privateKey;
            if (_fileCacheService.HasKey(CacheConstantKeys.TokenPrivateKey))
            {
                return File(_fileCacheService.GetFile(CacheConstantKeys.TokenPrivateKey), "application/x-pem-file", "private-key.pem");
            }
            else
            {
                var keys = await _downLoadFileService.GetTokenKeyFile();
                privateKey = keys[0];
                _fileCacheService.CacheFile(CacheConstantKeys.TokenPrivateKey, privateKey);
                return File(privateKey, "application/x-pem-file", "private-key.pem");
            }
        }

        /// <summary>
        /// 获取公钥文件
        /// </summary>
        /// <returns></returns>
        [HttpGet("publicKey")]
        public async Task<IActionResult> GetPublicKeyFile()
        {
            byte[]? publicKey;
            if (_fileCacheService.HasKey(CacheConstantKeys.TokenPrivateKey))
            {
                return File(_fileCacheService.GetFile(CacheConstantKeys.TokenPublicKey), "application/x-pem-file", "private-key.pem");
            }else
            {
                var keys = await _downLoadFileService.GetTokenKeyFile();
                publicKey = keys[1];
                _fileCacheService.CacheFile(CacheConstantKeys.TokenPublicKey, publicKey);
                return File(publicKey, "application/x-pem-file", "private-key.pem");
            }
        }

    }
}
