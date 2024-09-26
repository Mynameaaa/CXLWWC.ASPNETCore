using Microsoft.AspNetCore.Authorization;

namespace WWC._240711.ASPNETCore.Auth;

public class CXLAuthorizationEvaluator : IAuthorizationEvaluator
{
    /// <summary>
    /// 确定授权请求是否成功。
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public AuthorizationResult Evaluate(AuthorizationHandlerContext context)
    {
        if (!context.HasSucceeded)
        {
            return AuthorizationResult.Failed(context.HasFailed ? AuthorizationFailure.Failed(context.FailureReasons) : AuthorizationFailure.Failed(context.PendingRequirements));
        }

        return AuthorizationResult.Success();
    }
}