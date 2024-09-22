using Microsoft.Extensions.DependencyInjection;

namespace WWC._240711.ASPNETCore.Extensions;

public interface ICXLServiceContainer : IList<CXLServiceDescriptor>, IDisposable
{
    protected internal ICXLServiceContainer AddService(CXLServiceDescriptor serviceDescriptor);

    public bool SyncContainerServices(IServiceCollection services);

    public IServiceProvider Build(ICXLServiceContainer containerBuilder);

}
