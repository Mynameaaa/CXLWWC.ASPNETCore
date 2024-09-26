using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;


namespace WWC._240711.ASPNETCore.Auth;

public class CXLAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CXLAuthorizationMiddleware> _logger;
    private readonly IAuthorizationPolicyProvider _policyProvider;
    private readonly IAuthorizationService _authorizationService;

    public CXLAuthorizationMiddleware(
        RequestDelegate next,
        ILogger<CXLAuthorizationMiddleware> logger,
        IAuthorizationPolicyProvider policyProvider,
        IAuthorizationService authorizationService)
    {
        _next = next;
        _logger = logger;
        _policyProvider = policyProvider;
        _authorizationService = authorizationService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Step 1: 获取当前请求的终结点
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            _logger.LogInformation("未找到请求的终结点");
            await _next(context); // 无需授权，继续处理请求
            return;
        }

        // Step 2: 查找与终结点相关联的授权策略
        var authorizeData = endpoint.Metadata.GetOrderedMetadata<IAuthorizeData>();
        if (authorizeData == null || !authorizeData.Any())
        {
            _logger.LogInformation("终结点不包含授权数据");
            await _next(context); // 无需授权，继续处理请求
            return;
        }

        // Step 3: 通过策略提供器获取策略（使用自定义特性扫描）
        AuthorizationPolicy policy = await AuthorizationPolicy.CombineAsync(_policyProvider, authorizeData);
        if (policy == null)
        {
            _logger.LogError("未找到授权策略。");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        // Step 4: 执行授权
        var user = context.User;
        var authorizationResult = await _authorizationService.AuthorizeAsync(user, null, policy);

        if (authorizationResult.Succeeded)
        {
            _logger.LogInformation("Authorization successful.");
            await _next(context); // 授权成功，继续处理请求
        }
        else
        {
            _logger.LogWarning("Authorization failed.");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            // 这里可以自定义响应内容
            await context.Response.WriteAsync("Access Denied.");
        }
    }
}