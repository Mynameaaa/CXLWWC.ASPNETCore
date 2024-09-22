namespace WWC._240711.ASPNETCore.Extensions;

public static class CXLServiceProviderExtensions
{
    public static object GetRequiredService(this IServiceProvider provider, Type type)
    {
        if (provider == null) throw new ArgumentNullException(nameof(provider));
        if (provider.TryGetService(type, out object instance))
        {
            return instance;
        }
        else
        {
            throw new InvalidOperationException("无法获取该服务");
        }
    }

    //public static object Init

    public static CXLScoped CreateScopeA(this IServiceProvider serviceProvider)
    {
        return new CXLScoped(serviceProvider);
    }

    public static bool TryGetService(this IServiceProvider provider, Type type, out object instance)
    {
        instance = provider.GetService(type)!;
        if (instance == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
