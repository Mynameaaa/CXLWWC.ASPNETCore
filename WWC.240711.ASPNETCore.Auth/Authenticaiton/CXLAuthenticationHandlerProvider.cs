using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace WWC._240711.ASPNETCore.Auth;

/// <summary>
/// 自定义鉴权处理提供者
/// </summary>
public class CXLAuthenticationHandlerProvider : IAuthenticationHandlerProvider
{
    //处理程序实例缓存，每个请求需要初始化一次
    private readonly Dictionary<string, IAuthenticationHandler> _handlerMap = new Dictionary<string, IAuthenticationHandler>(StringComparer.Ordinal);

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="schemes">The <see cref="IAuthenticationHandlerProvider"/>.</param>
    public CXLAuthenticationHandlerProvider(IAuthenticationSchemeProvider schemes)
    {
        Schemes = schemes;
    }

    /// <summary>
    /// The <see cref="IAuthenticationHandlerProvider"/>.
    /// </summary>
    public IAuthenticationSchemeProvider Schemes { get; }

    public async Task<IAuthenticationHandler?> GetHandlerAsync(HttpContext context, string authenticationScheme)
    {
        //先查看缓存中有没有
        if (_handlerMap.TryGetValue(authenticationScheme, out var cacheHandler))
        {
            return cacheHandler;
        }

        //获取策略提供者
        var schemeProvider = Schemes;

        //获取全部鉴权策略
        var currentScheme = await schemeProvider.GetSchemeAsync(authenticationScheme);

        if (currentScheme != null)
        {
            var handler = (context.RequestServices.GetService(currentScheme.HandlerType) ??
            ActivatorUtilities.CreateInstance(context.RequestServices, currentScheme.HandlerType))
            as IAuthenticationHandler;
            if (handler == null)
            {
                throw new InvalidOperationException($"AuthenticationHandler 异常：{currentScheme.HandlerType.Name}，无法获取该类型的服务");
            }
            _handlerMap[authenticationScheme] = handler;
            await handler.InitializeAsync(currentScheme, context);
            return handler;
        }

        var schemes = await schemeProvider.GetAllSchemesAsync();

        //获取标记为默认的 Handler 
        var defaultSchemes = schemes.Where(p => p.HandlerType.GetCustomAttribute(typeof(NoSchemeDefaultHandlerAttribute)) != null).FirstOrDefault() ?? schemes.FirstOrDefault(p => p.Name
         .Equals(CXLConstantScheme.DefaultScheme));

        if (defaultSchemes != null)
        {
            var handler = (IAuthenticationHandler?)context.RequestServices.GetService(defaultSchemes.HandlerType);
            _handlerMap[authenticationScheme] = handler;
            if (handler == null)
            {
                throw new InvalidOperationException($"AuthenticationHandler 异常：{defaultSchemes.HandlerType.Name}，无法获取该类型的服务");
            }
            //一定要记得调用这个方法这是为了初始化鉴权信息
            await handler.InitializeAsync(defaultSchemes, context);
            return handler;
        }
        else
        {
            throw new Exception($"不存在名称为 [{authenticationScheme}] 的 Schemes，并且不包含拥有 NoSchemeDefaultHandlerAttribute 特性的 Handler");
        }

    }
}
