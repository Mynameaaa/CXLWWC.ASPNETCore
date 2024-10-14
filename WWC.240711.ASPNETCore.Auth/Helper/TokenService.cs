using System.Security.Claims;
using WWC._240711.ASPNETCore.Infrastructure;

namespace WWC._240711.ASPNETCore.Auth;

public class TokenService : ITokenService
{
    private readonly ITokenHelper _tokenHelper;
    private Dictionary<string, RefreshTokenCacheModel> _refreshTokens = new Dictionary<string, RefreshTokenCacheModel>();

    public TokenService(ITokenHelper tokenHelper)
    {
        _tokenHelper = tokenHelper;
    }

    /// <summary>
    /// 获取 Token 对
    /// </summary>
    /// <param name="privateKeyPath"></param>
    /// <param name="tokenMinute"></param>
    /// <param name="dataModel"></param>
    /// <returns></returns>
    public async Task<TokenModel> GenerateJwtToken(string privateKeyPath, Dictionary<string, string> dataModel = null)
    {
        var accessToken = await _tokenHelper.GenerateJwtToken(privateKeyPath, dataModel);
        var refreshToken = _tokenHelper.GenerateRefreshToken();
        _refreshTokens.Add(refreshToken, new RefreshTokenCacheModel()
        {
            Expiration = DateTime.Now.AddMinutes(Appsettings.app<int?>("TokenParameter:RefreshTokenExpiration") ?? 2880),
            UserID = dataModel["ID"],
        });

        return new TokenModel()
        {
            access_token = accessToken,
            refresh_token = refreshToken,
        };
    }

    /// <summary>
    /// 获取 Token 对
    /// </summary>
    /// <param name="privateKeyPath"></param>
    /// <param name="tokenMinute"></param>
    /// <param name="dataModel"></param>
    /// <returns></returns>
    public TokenModel GenerateJwtToken(byte[] privateKey, Dictionary<string, string> dataModel = null)
    {
        var accessToken = _tokenHelper.GenerateJwtToken(privateKey, dataModel);
        var refreshToken = _tokenHelper.GenerateRefreshToken();
        _refreshTokens.Add(refreshToken, new RefreshTokenCacheModel()
        {
            Expiration = DateTime.Now.AddMinutes(Appsettings.app<int?>("TokenParameter:RefreshTokenExpiration") ?? 2880),
            UserID = dataModel["userId"],
        });

        return new TokenModel()
        {
            access_token = accessToken,
            refresh_token = refreshToken,
        };
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Dictionary<string, string> GetClaimsFromRefreshToken(string refreshToken)
    {
        if (!_refreshTokens.ContainsKey(refreshToken))
            return null;

        var userData = _refreshTokens[refreshToken];

        // 从数据库或其他地方获取用户信息
        var user = UserData.UserList.FirstOrDefault(p => p.UserID == int.Parse(userData.UserID));

        if (user == null)
        {
            return null;
        }

        // 创建 ClaimsIdentity
        var claims = new List<Claim>();

        Dictionary<string, string> dataModels = new Dictionary<string, string>
        {
            { "Name", user.UserName },
            { "ID", user.UserID.ToString() },
            { "Role", user.Role }
        };

        this.DeleteInvalidRefreshToken(refreshToken);

        return dataModels;
    }

    /// <summary>
    /// 验证刷新 Token
    /// </summary>
    /// <param name="refresh_token"></param>
    /// <returns></returns>
    public bool ValidationRefreshToken(string refresh_token)
    {
        if (!_refreshTokens.ContainsKey(refresh_token))
            return false;

        var time = _refreshTokens[refresh_token];
        if (time.Expiration > DateTime.Now)
            return true;

        return false;
    }

    /// <summary>
    /// 删除已经刷新过的 refresh_Token
    /// </summary>
    /// <param name="refresh_token"></param>
    /// <returns></returns>
    private bool DeleteInvalidRefreshToken(string refresh_token)
    {
        return _refreshTokens.Remove(refresh_token);
    }

}
