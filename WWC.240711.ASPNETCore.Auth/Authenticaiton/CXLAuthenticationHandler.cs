using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using WWC._240711.ASPNETCore.Infrastructure;

namespace WWC._240711.ASPNETCore.Auth;

[NoSchemeDefaultHandler]
public class CXLAuthenticationHandler : AuthenticationHandler<CXLAuthenticationSchemeOptions>
{

    private readonly ITokenService _tokenService;

    /// <summary>
    /// 鉴权选项
    /// </summary>
    private readonly IOptionsMonitor<CXLAuthenticationSchemeOptions> _options;

    /// <summary>
    /// 日志工厂
    /// </summary>
    private readonly ILoggerFactory _logger;

    /// <summary>
    /// Url 编码
    /// </summary>
    private readonly UrlEncoder _urlEncoder;


    private readonly ISystemClock _clock;

    /// <summary>
    /// 发出质询返回的消息
    /// </summary>
    private Dictionary<string, string> _challengeMessages = new Dictionary<string, string>();

    public CXLAuthenticationHandler(IOptionsMonitor<CXLAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, ITokenService tokenService) : base(options, logger, encoder, clock)
    {
        _options = options;
        _logger = logger;
        _urlEncoder = UrlEncoder;
        _clock = clock;
        _tokenService = tokenService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        AuthenticateResult result = default(AuthenticateResult);

        var options = base.Options;

        #region 支持自定义鉴权策略

        if (options.UseEventResult)
        {
            var EventResult = options.InvokeAuthEvent(_logger);
            if (EventResult != null)
                return await HandlerEventAuthentication(EventResult);
        }

        #endregion

        // 加载 access_token
        var token = InitToken("Authorization");

        var useRefreshToken = Appsettings.app<bool?>("UseRefreshToken") ?? false;

        if ((!token.Item1 || string.IsNullOrWhiteSpace(token.Item2)) && useRefreshToken)
        {
            return await this.HandlerUseRefreshTokenAuth();
        }
        else
        {
            // 验证 access_token 是否有效
            try
            {
                var accessTokenPrincipal = ValidateToken(token.Item2);

                if (accessTokenPrincipal == null)
                {
                    throw new SecurityTokenExpiredException();
                }

                // 构建 AuthenticationTicket
                var authenticationTicket = new AuthenticationTicket(accessTokenPrincipal, Scheme.Name);
                return AuthenticateResult.Success(authenticationTicket);
            }
            catch (SecurityTokenExpiredException)
            {
                if (!useRefreshToken)
                {
                    _challengeMessages.Add("message", "Token 已过期请重新登录");
                    return AuthenticateResult.Fail(string.Empty);
                }

                return await this.HandlerUseRefreshTokenAuth();
            }
        }
    }

    private async Task<AuthenticateResult> HandlerUseRefreshTokenAuth()
    {
        // access_token 无效，检查是否存在 refresh_token
        var refreshToken = InitToken("refresh_token");

        if (!refreshToken.Item1 || string.IsNullOrWhiteSpace(refreshToken.Item2))
        {
            // 两者都无效，返回 401
            _challengeMessages.Add("message", "无效的 Token 请重新登录");
            return AuthenticateResult.Fail(string.Empty);
        }

        // 验证 refresh_token
        bool isRefreshTokenValid = _tokenService.ValidationRefreshToken(refreshToken.Item2);

        if (isRefreshTokenValid)
        {
            // 从 refresh_token 中获取用户信息或 Claims 信息
            var userClaims = _tokenService.GetClaimsFromRefreshToken(refreshToken.Item2);

            if (userClaims == null)
            {
                throw new Exception("异常的 Claims 信息");
            }

            // 生成新的 access_token 和 refresh_token
            string privateKeyPath = Path.Combine(Directory.GetCurrentDirectory(), Appsettings.app("TokenKey:PrivateKeyPath") ?? "Keys/public.pem");

            var newTokenModel = await _tokenService.GenerateJwtToken(privateKeyPath, userClaims);
            var newAccessToken = newTokenModel.access_token;
            var newRefreshToken = newTokenModel.refresh_token;

            // 返回新的令牌
            base.Context.Response.Headers["Authorization"] = "Bearer " + newAccessToken;
            base.Context.Response.Headers["refresh_token"] = newRefreshToken;

            // 验证新的 access_token
            var newPrincipal = ValidateToken(newAccessToken);
            if (newPrincipal != null)
            {
                var newAuthenticationTicket = new AuthenticationTicket(newPrincipal, Scheme.Name);
                return AuthenticateResult.Success(newAuthenticationTicket);
            }
            else
            {
                _challengeMessages.Add("message", "新生成的 Token 验证失败");
                return AuthenticateResult.Fail(string.Empty);
            }

        }

        _challengeMessages.Add("message", "无效的 refersh_Token");
        return AuthenticateResult.Fail(string.Empty);
    }

    private async Task<AuthenticateResult> HandlerEventAuthentication(Task<AuthenticateResult> eventResult)
    {
        var Log = _logger.CreateLogger<CXLAuthenticationHandler>();
        Log.LogInformation("执行了由事件触发的鉴权策略");
        var result = await eventResult;
        return result;
    }

    /// <summary>
    /// 获取 Token
    /// </summary>
    /// <returns></returns>
    private (bool, string) InitToken(string tokenHeader)
    {
        string? token = string.Empty;

        token = base.Context.Request.Headers[tokenHeader];
        if (string.IsNullOrWhiteSpace(token))
            return (false, "不存在 Token 信息");

        // 使用不区分大小写的方式匹配 "Bearer "
        var bearerPrefix = "Bearer ";
        if (token.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            string newToken = token.Substring(bearerPrefix.Length).Trim();
            token = newToken;
        }

        return (true, token);
    }

    private ClaimsPrincipal ValidateToken(string Token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = base.Options.SecretKey, // 设置签名密钥
            ValidateIssuer = base.Options.ValidateIssuer,  // 验证发行者
            ValidateAudience = base.Options.ValidateAudience,  // 验证观众
            ValidAudience = base.Options.Audience,
            ValidIssuer = base.Options.Issuer,
            ValidateLifetime = true, // 验证生命周期
            ClockSkew = TimeSpan.FromSeconds(30) // 时钟偏移
        };

        SecurityToken validatedToken;
        var principal = tokenHandler.ValidateToken(Token, validationParameters, out validatedToken);
        return principal;
    }

    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        // 设置 401 状态码
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.ContentType = "application/json";
        return base.HandleForbiddenAsync(properties);
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        // 设置 401 状态码
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.ContentType = "application/json";

        // 检查是否存在 "Message" 参数，否则使用默认消息
        var message = properties?.Parameters.ContainsKey("Message") == true
            ? properties.Parameters["Message"]?.ToString()
            : "Unauthorized";

        var jsonResponse = new Dictionary<string, string>
        {
            { "systemMessage", message },
            { "redirectUrl", Options.RedirectUrl },
        };

        if (!_challengeMessages.Any())
            jsonResponse.Add("message", Options.DefualtChallageMessage);
        else
        {
            foreach (var messageKeyValue in _challengeMessages)
            {
                jsonResponse[messageKeyValue.Key] = messageKeyValue.Value;
            }
        }

        var jsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(jsonResponse);
        await Response.WriteAsync(jsonResult);
    }

    /// <summary>
    /// 初始化 Handler 信息
    /// </summary>
    /// <returns></returns>
    protected override Task InitializeHandlerAsync()
    {
        return base.InitializeHandlerAsync();
    }

}
