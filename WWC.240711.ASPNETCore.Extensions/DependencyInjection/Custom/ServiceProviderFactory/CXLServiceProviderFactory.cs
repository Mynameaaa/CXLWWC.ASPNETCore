using Microsoft.Extensions.DependencyInjection;

namespace WWC._240711.ASPNETCore.Extensions;

public class CXLServiceProviderFactory : IServiceProviderFactory<ICXLServiceContainer>
{
    private readonly Action<ICXLServiceContainer> _configurationAction;

    public CXLServiceProviderFactory(Action<ICXLServiceContainer> configurationAction = null)
    {
        _configurationAction = configurationAction ?? (builder => { });
    }

    public ICXLServiceContainer CreateBuilder(IServiceCollection services)
    {
        var containerBuilder = new CXLServiceContainer();
        containerBuilder.SyncContainerServices(services);

        _configurationAction.Invoke(containerBuilder);
        containerBuilder.AddService(new CXLServiceDescriptor(typeof(ICXLServiceContainer), typeof(CXLServiceContainer), CXLServiceLifetime.Singleton) { Instance = containerBuilder });
        return containerBuilder;
    }

    public IServiceProvider CreateServiceProvider(ICXLServiceContainer containerBuilder)
    {
        if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));
        //获取Container容器，因为接下来要使用获取实例的方法了
        var container = containerBuilder.Build(containerBuilder);
        return new CXLServiceProvider(containerBuilder);
    }
}
