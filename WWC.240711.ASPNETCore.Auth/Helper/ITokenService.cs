namespace WWC._240711.ASPNETCore.Auth;

public interface ITokenService
{
    /// <summary>
    /// 获取 Token 对
    /// </summary>
    /// <param name="privateKeyPath"></param>
    /// <param name="tokenMinute"></param>
    /// <param name="dataModel"></param>
    /// <returns></returns>
    Task<TokenModel> GenerateJwtToken(string privateKeyPath, Dictionary<string, string> dataModel = null);

    /// <summary>
    /// 验证刷新 Token
    /// </summary>
    /// <param name="refresh_token"></param>
    /// <returns></returns>
    bool ValidationRefreshToken(string refresh_token);

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Dictionary<string,string> GetClaimsFromRefreshToken(string userId);

}
