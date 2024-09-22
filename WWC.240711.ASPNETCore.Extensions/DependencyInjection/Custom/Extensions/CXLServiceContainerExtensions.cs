using System.Runtime.CompilerServices;

namespace WWC._240711.ASPNETCore.Extensions;

public static class CXLServiceContainerExtensions
{

    /// <summary>
    /// 添加单例服务
    /// </summary>
    /// <returns></returns>
    public static ICXLServiceContainer AddSingleton(this ICXLServiceContainer container, Type implementationType)
    {
        return container.AddSingleton(implementationType, implementationType);
    }

    /// <summary>
    /// 添加单例服务
    /// </summary>
    /// <returns></returns>
    public static ICXLServiceContainer AddSingleton<T, T2>(this ICXLServiceContainer container, Func<IServiceProvider, T> instanceFactory) where T2 : T where T : class
    {
        return container.AddService(new CXLServiceDescriptor(typeof(T), typeof(T2), CXLServiceLifetime.Singleton)
        {
            InstanceServiceFactory = instanceFactory
        });
    }


    /// <summary>
    /// 添加单例服务
    /// </summary>
    /// <returns></returns>
    public static ICXLServiceContainer AddSingleton<T>(this ICXLServiceContainer container, Func<IServiceProvider, T> instanceFactory) where T : class
    {
        return container.AddService(new CXLServiceDescriptor(typeof(T), typeof(T), CXLServiceLifetime.Singleton)
        {
            InstanceServiceFactory = instanceFactory
        });
    }

    /// <summary>
    /// 添加单例服务
    /// </summary>
    /// <returns></returns>
    public static ICXLServiceContainer AddSingleton(this ICXLServiceContainer container, Type interfaces, Type implementationType)
    {
        ThrowContainerExecption(container);
        ThrowSerivceExecption(implementationType);
        return container.AddService(new CXLServiceDescriptor(interfaces, implementationType, CXLServiceLifetime.Singleton));
    }

    public static ICXLServiceContainer AddService(this ICXLServiceContainer container, CXLServiceDescriptor serviceDescriptor)
    {
        return container.AddService(new CXLServiceDescriptor(serviceDescriptor.InterfacesType, serviceDescriptor.ImplementationType, CXLServiceLifetime.Singleton));
    }

    /// <summary>
    /// 添加单例服务
    /// </summary>
    /// <returns></returns>
    public static ICXLServiceContainer AddScoped(this ICXLServiceContainer container, Type implementationType)
    {
        return container.AddScoped(implementationType, implementationType);
    }

    /// <summary>
    /// 添加单例服务
    /// </summary>
    /// <returns></returns>
    public static ICXLServiceContainer AddScoped(this ICXLServiceContainer container, Type interfaces, Type implementationType)
    {
        ThrowContainerExecption(container);
        ThrowSerivceExecption(implementationType);
        return container.AddService(new CXLServiceDescriptor(interfaces, implementationType, CXLServiceLifetime.Scoped));
    }

    /// <summary>
    /// 添加单例服务
    /// </summary>
    /// <returns></returns>
    public static ICXLServiceContainer AddTransient(this ICXLServiceContainer container, Type implementationType)
    {
        return container.AddTransient(implementationType, implementationType);
    }


    /// <summary>
    /// 添加单例服务
    /// </summary>
    /// <returns></returns>
    public static ICXLServiceContainer AddTransient(this ICXLServiceContainer container, Type interfaces, Type implementationType)
    {
        ThrowContainerExecption(container);
        ThrowSerivceExecption(implementationType);
        return container.AddService(new CXLServiceDescriptor(interfaces, implementationType, CXLServiceLifetime.Singleton));
    }

    public static void ThrowContainerExecption(ICXLServiceContainer container)
    {
        if (container == null) throw new ArgumentNullException($"容器 {container} 为空无法注册服务！");
    }

    public static void ThrowSerivceExecption(Type implementationType)
    {
        if (implementationType == null) throw new ArgumentNullException($"无法注册类型为空的 {implementationType} 服务！");
    }

}
