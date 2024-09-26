using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace WWC._240711.ASPNETCore.Auth;

public class CXLAuthorizationService : IAuthorizationService
{
    private readonly AuthorizationOptions _options;

    private readonly IAuthorizationHandlerContextFactory _contextFactory;

    private readonly IAuthorizationHandlerProvider _handlerProvider;

    private readonly IAuthorizationEvaluator _evaluator;

    private readonly IAuthorizationPolicyProvider _policyProvider;

    private readonly ILogger _logger;

    //
    // 摘要:
    //      创建Microsoft的新实例。默认授权服务。
    //
    // 参数:
    //      策略提供者:
    //      IAuthorizationPolicyProvider 用于提供策略
    //
    //      处理程序:
    //      IAuthorizationRequirements.
    //
    //      记录器:
    //      用于记录消息、警告和错误的记录器。.
    //
    //      上下文工厂:
    //      IAuthorizationHandlerContextFactory 创建处理授权的上下文。
    //
    //      评估员:
    //      IAuthorizationEvaluator 用于确定如果授权成功。
    //
    //      选项:
    //      IOptions<AuthorizationOptions> 使用的授权选项。
    public CXLAuthorizationService(IAuthorizationPolicyProvider policyProvider, IAuthorizationHandlerProvider handlerProvider, ILogger<DefaultAuthorizationService> logger, IAuthorizationHandlerContextFactory contextFactory, IAuthorizationEvaluator evaluator, IOptions<AuthorizationOptions> options)
    {
        _handlerProvider = handlerProvider;
        _policyProvider = policyProvider;
        _logger = logger;
        _evaluator = evaluator;
        _contextFactory = contextFactory;
        _options = options.Value;
    }

    //
    // 摘要:
    //     检查用户是否满足指定资源的特定要求集。
    //
    // 参数:
    //      user:
    //      用户的票据信息
    //
    //      resource:
    //      用于评估需求的资源。
    //
    //      requirements:
    //      评估的要求。
    //
    // 返回结果:
    //      指示授权是否成功的标志。当满足以下条件时，此值为 true
    //      用户满足策略，否则为false。
    public virtual async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, IEnumerable<IAuthorizationRequirement> requirements)
    {
        AuthorizationHandlerContext authContext = _contextFactory.CreateContext(requirements, user, resource);
        var handlers = _handlerProvider.GetHandlersAsync(authContext).ConfigureAwait(continueOnCapturedContext: false);
        foreach (IAuthorizationHandler handler in await handlers)
        {
            await handler.HandleAsync(authContext).ConfigureAwait(continueOnCapturedContext: false);
            if (!_options.InvokeHandlersAfterFailure && authContext.HasFailed)
            {
                break;
            }
        }

        AuthorizationResult authorizationResult = _evaluator.Evaluate(authContext);
        if (authorizationResult.Succeeded)
        {
            Console.WriteLine("授权成功标识");
        }
        else
        {
            Console.WriteLine("授权失败标识");
        }

        return authorizationResult;
    }

    //
    // 摘要:
    //     检查用户是否符合特定的授权策略。
    //
    // 参数:
    //      user:
    //      用户的票据信息
    //
    //      resource:
    //      用于评估需求的资源。
    //
    //      policyName:
    //      根据特定上下文检查的策略名称。
    //
    // 返回结果:
    //      指示授权是否成功的标志。当满足以下条件时，此值为 true
    //      用户满足策略，否则为false。
    public virtual async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName)
    {
        AuthorizationPolicy authorizationPolicy = await _policyProvider.GetPolicyAsync(policyName).ConfigureAwait(continueOnCapturedContext: false);
        if (authorizationPolicy == null)
        {
            throw new InvalidOperationException("No policy found: " + policyName + ".");
        }

        return await this.AuthorizeAsync(user, resource, authorizationPolicy).ConfigureAwait(continueOnCapturedContext: false);
    }
}