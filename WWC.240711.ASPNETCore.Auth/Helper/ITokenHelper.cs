using System.Security.Claims;

namespace WWC._240711.ASPNETCore.Auth;
public interface ITokenHelper
{
    /// <summary>
    /// 生成 access_Token
    /// </summary>
    /// <param name="privateKeyPath"></param>
    /// <param name="minute"></param>
    /// <param name="dataModel"></param>
    /// <returns></returns>
    Task<string> GenerateJwtToken(string privateKeyPath, Dictionary<string, string> dataModel = null);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    string GenerateRefreshToken();


    /// <summary>
    /// 验证 JWT 令牌
    /// </summary>
    /// <param name="token"></param>
    /// <param name="publicKeyPath"></param>
    /// <returns></returns>
    Task<ClaimsPrincipal> ValidateToken(string token, string publicKeyPath);

}
