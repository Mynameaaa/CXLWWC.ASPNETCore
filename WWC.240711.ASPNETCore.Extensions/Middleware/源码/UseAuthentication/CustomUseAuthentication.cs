////授权给。NET Foundation在一个或多个协议下。
////那个。NET Foundation根据MIT许可证将此文件授权给您。
//using System;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http.Features.Authentication;
//using Microsoft.Extensions.DependencyInjection;

//namespace Microsoft.AspNetCore.Authentication
//{
//    public class AuthenticationMiddleware
//    {
//        private readonly RequestDelegate _next;

//        public AuthenticationMiddleware(RequestDelegate next, IAuthenticationSchemeProvider schemes)
//        {
//            if (next == null)
//            {
//                throw new ArgumentNullException(nameof(next));
//            }
//            if (schemes == null)
//            {
//                throw new ArgumentNullException(nameof(schemes));
//            }

//            _next = next;
//            Schemes = schemes;
//        }

//        public IAuthenticationSchemeProvider Schemes { get; set; }

//        public async Task Invoke(HttpContext context)
//        {
//            context.Features.Set<IAuthenticationFeature>(new AuthenticationFeature
//            {
//                OriginalPath = context.Request.Path,
//                OriginalPathBase = context.Request.PathBase
//            });

//            // 获取所有请求处理程序方案，并尝试处理请求
//            var handlers = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
//            foreach (var scheme in await Schemes.GetRequestHandlerSchemesAsync())
//            {
//                var handler = await handlers.GetHandlerAsync(context, scheme.Name) as IAuthenticationRequestHandler;
//                if (handler != null && await handler.HandleRequestAsync())
//                {
//                    return; // 如果某个处理程序处理了请求，则返回
//                }
//            }

//            // 获取默认的身份验证方案并执行身份验证
//            var defaultAuthenticate = await Schemes.GetDefaultAuthenticateSchemeAsync();
//            if (defaultAuthenticate != null)
//            {
//                var result = await context.AuthenticateAsync(defaultAuthenticate.Name);
//                if (result?.Principal != null)
//                {
//                    context.User = result.Principal; // 设置当前用户
//                }
//                if (result?.Succeeded ?? false)
//                {
//                    var authFeatures = new AuthenticationFeatures(result);
//                    context.Features.Set<IHttpAuthenticationFeature>(authFeatures);
//                    context.Features.Set<IAuthenticateResultFeature>(authFeatures);
//                }
//            }

//            // 调用下一个中间件
//            await _next(context);
//        }
//    }

//}
