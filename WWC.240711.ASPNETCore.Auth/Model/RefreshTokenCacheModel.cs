namespace WWC._240711.ASPNETCore.Auth;

public class RefreshTokenCacheModel
{
    /// <summary>
    /// 有效期
    /// </summary>
    public DateTime Expiration { get; set; }

    /// <summary>
    /// 用户编号
    /// </summary>
    public string UserID { get; set; }

}
