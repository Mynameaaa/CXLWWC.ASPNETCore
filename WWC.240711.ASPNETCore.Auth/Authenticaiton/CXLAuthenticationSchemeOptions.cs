using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace WWC._240711.ASPNETCore.Auth;

public class CXLAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public const string SchemeName = "CXL";

    public string DisplayName { get; set; }

    public int Age { get; set; }

    public bool ValidateAudience { get; set; }

    public bool ValidateSecretKey { get; set; }

    public string Audience { get; set; }

    public bool ValidateIssuer { get; set; }

    public string Issuer { get; set; }

    public bool UseEventResult { get; set; }

    public SecurityKey SecretKey { get; set; }

    public string RedirectUrl { get; set; }

    public string DefualtChallageMessage { get; set; }

    public string DefualtForbirdMessage { get; set; }


    public delegate Task<AuthenticateResult> authenticationHandler(ILoggerFactory logger);


    public event authenticationHandler AuthEvent;

    public Task<AuthenticateResult> InvokeAuthEvent(ILoggerFactory logger)
    {
        if (AuthEvent != null)
        {
            return AuthEvent(logger);
        }
        else
        {
            return null;
        }
    }

}
