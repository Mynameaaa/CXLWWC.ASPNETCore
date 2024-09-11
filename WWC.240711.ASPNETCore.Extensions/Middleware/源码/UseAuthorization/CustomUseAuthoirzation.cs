//using System;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authorization.Policy;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http.Features.Authentication;
//using Microsoft.Extensions.DependencyInjection;

//namespace Microsoft.AspNetCore.Authorization
//{
//    /// <summary>
//    /// 一种支持授权功能的中间件。
//    /// </summary>
//    public class AuthorizationMiddleware
//    {
//        // AppContext开关用于控制是将HttpContext还是端点作为资源传递给AuthZ
//        private const string SuppressUseHttpContextAsAuthorizationResource = "Microsoft.AspNetCore.Authorization.SuppressUseHttpContextAsAuthorizationResource";

//        // 端点路由使用属性密钥来确定授权是否已运行
//        private const string AuthorizationMiddlewareInvokedWithEndpointKey = "__AuthorizationMiddlewareWithEndpointInvoked";
//        private static readonly object AuthorizationMiddlewareWithEndpointInvokedValue = new object();

//        private readonly RequestDelegate _next;
//        private readonly IAuthorizationPolicyProvider _policyProvider;

//        /// <summary>
//        /// 初始化<see cref=“AuthorizationMiddleware”/>的新实例。
//        /// </summary>
//        /// <param name="next">应用程序中间件管道中的下一个中间件.</param>
//        /// <param name="policyProvider">这个 <see cref="IAuthorizationPolicyProvider"/>.</param>
//        public AuthorizationMiddleware(RequestDelegate next, IAuthorizationPolicyProvider policyProvider)
//        {
//            _next = next ?? throw new ArgumentNullException(nameof(next));
//            _policyProvider = policyProvider ?? throw new ArgumentNullException(nameof(policyProvider));
//        }

//        /// <summary>
//        /// 调用执行授权的中间件。
//        /// </summary>
//        /// <param name="context">The <see cref="HttpContext"/>.</param>
//        public async Task Invoke(HttpContext context)
//        {
//            if (context == null)
//            {
//                throw new ArgumentNullException(nameof(context));
//            }

//            var endpoint = context.GetEndpoint();

//            if (endpoint != null)
//            {
//                // EndpointRoutingMiddleware使用此标志检查授权中间件是否处理了端点上的身份验证元数据。
//                // 授权中间件只有在观察到实际端点时才能做出此声明。
//                context.Items[AuthorizationMiddlewareInvokedWithEndpointKey] = AuthorizationMiddlewareWithEndpointInvokedValue;
//            }

//            // 重要提示：授权逻辑的更改应反映在MVC的AuthorizeFilter中
//            var authorizeData = endpoint?.Metadata.GetOrderedMetadata<IAuthorizeData>() ?? Array.Empty<IAuthorizeData>();
//            var policy = await AuthorizationPolicy.CombineAsync(_policyProvider, authorizeData);
//            if (policy == null)
//            {
//                await _next(context);
//                return;
//            }

//            // 策略评估器具有瞬态生存期，因此它是从请求服务中获取的，而不是注入构造函数
//            var policyEvaluator = context.RequestServices.GetRequiredService<IPolicyEvaluator>();

//            var authenticateResult = await policyEvaluator.AuthenticateAsync(policy, context);

//            if (authenticateResult?.Succeeded ?? false)
//            {
//                if (context.Features.Get<IAuthenticateResultFeature>() is IAuthenticateResultFeature authenticateResultFeature)
//                {
//                    authenticateResultFeature.AuthenticateResult = authenticateResult;
//                }
//                else
//                {
//                    var authFeatures = new AuthenticationFeatures(authenticateResult);
//                    context.Features.Set<IHttpAuthenticationFeature>(authFeatures);
//                    context.Features.Set<IAuthenticateResultFeature>(authFeatures);
//                }
//            }

//            // AllowAnonymousAttribute 可以用于跳过授权中间件
//            if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
//            {
//                await _next(context);
//                return;
//            }

//            object? resource;
//            if (AppContext.TryGetSwitch(SuppressUseHttpContextAsAuthorizationResource, out var useEndpointAsResource) && useEndpointAsResource)
//            {
//                resource = endpoint;
//            }
//            else
//            {
//                resource = context;
//            }

//            var authorizeResult = await policyEvaluator.AuthorizeAsync(policy, authenticateResult!, context, resource);
//            var authorizationMiddlewareResultHandler = context.RequestServices.GetRequiredService<IAuthorizationMiddlewareResultHandler>();
//            await authorizationMiddlewareResultHandler.HandleAsync(_next, context, policy, authorizeResult);
//        }
//    }
//}
