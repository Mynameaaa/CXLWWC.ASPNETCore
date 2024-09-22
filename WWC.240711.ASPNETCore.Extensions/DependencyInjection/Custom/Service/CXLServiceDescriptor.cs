
namespace WWC._240711.ASPNETCore.Extensions;

public class CXLServiceDescriptor
{
    public Type InterfacesType { get; init; }

    public Type ImplementationType { get; init; }

    public CXLServiceLifetime Lifetime { get; init; }

    public Func<IServiceProvider, object> InstanceServiceFactory { get; set; }

    public object Instance { get; set; }

    //ServiceLifetime

    public CXLServiceDescriptor(Type interfacesType, Type implementationType, CXLServiceLifetime lifetime)
    {
        InterfacesType = interfacesType;
        this.ImplementationType = implementationType;
        Lifetime = lifetime;
    }
}
