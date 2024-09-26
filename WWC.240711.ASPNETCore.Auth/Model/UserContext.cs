using System.Security.Claims;

namespace WWC._240711.ASPNETCore.Auth;

public class UserContext
{
    public static TokenDataModel TokenDataModel { get; set; } = new TokenDataModel();

    public static string UserName => TokenDataModel.Username;

    public static string Role => TokenDataModel.Role;

    public static void SetContext(List<Claim>? claims)
    {
        if (claims == null)
            return;

        TokenDataModel.Role = claims.FirstOrDefault(p => p.Type == "Role")?.Value ?? string.Empty;
        TokenDataModel.Username = claims.FirstOrDefault(p => p.Type == "Name")?.Value ?? string.Empty;
    }

}
