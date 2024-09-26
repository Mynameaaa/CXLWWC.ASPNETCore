namespace WWC._240711.ASPNETCore.Auth;

public class TokenModel
{
    /// <summary>
    /// 访问令牌
    /// </summary>
    public string access_token { get; set; }

    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string refresh_token { get; set; }

}
