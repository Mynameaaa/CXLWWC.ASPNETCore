using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace WWC._240711.ASPNETCore.Extensions;

public class CXLServiceContainer : ICXLServiceContainer
{
    private IList<CXLServiceDescriptor> _serviceCollection;

    public ICXLServiceSubProvider SubContainer { get; init; }

    public CXLServiceContainer()
    {
        _serviceCollection = new List<CXLServiceDescriptor>();
    }

    public CXLServiceDescriptor this[int index] { get => _serviceCollection[index]; set => _serviceCollection[index] = value; }

    public int Count => _serviceCollection.Count;

    public bool IsReadOnly => _serviceCollection.IsReadOnly;

    public void Add(CXLServiceDescriptor item)
    {
        _serviceCollection.Add(item);
    }

    ICXLServiceContainer ICXLServiceContainer.AddService(CXLServiceDescriptor serviceDescriptor)
    {
        this.Add(serviceDescriptor);
        return this;
    }

    internal ICXLServiceContainer AddService(CXLServiceDescriptor serviceDescriptor)
    {
        this.Add(serviceDescriptor);
        return this;
    }

    public void Clear()
    {
        _serviceCollection.Clear();
    }

    public bool Contains(CXLServiceDescriptor item)
    {
        return _serviceCollection.Contains(item);
    }

    public void CopyTo(CXLServiceDescriptor[] array, int arrayIndex)
    {
        _serviceCollection.CopyTo(array, arrayIndex);
    }

    public IEnumerator<CXLServiceDescriptor> GetEnumerator()
    {
        return _serviceCollection.GetEnumerator();
    }

    public int IndexOf(CXLServiceDescriptor item)
    {
        return _serviceCollection.IndexOf(item);
    }

    public void Insert(int index, CXLServiceDescriptor item)
    {
        _serviceCollection.Insert(index, item);
    }

    public bool Remove(CXLServiceDescriptor item)
    {
        return _serviceCollection.Remove(item);
    }

    public void RemoveAt(int index)
    {
        _serviceCollection.RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_serviceCollection).GetEnumerator();
    }

    public void Dispose()
    {

    }

    public bool SyncContainerServices(IServiceCollection serviceDescirptors)
    {
        foreach (var descriptor in serviceDescirptors)
        {
            this.Add(new CXLServiceDescriptor(descriptor.ServiceType, descriptor.ImplementationType, (CXLServiceLifetime)descriptor.Lifetime)
            {
                Instance = descriptor.ImplementationInstance ?? default(object),
                InstanceServiceFactory = descriptor.ImplementationFactory ?? default(Func<IServiceProvider, object>),
            });
        }

        return true;
    }

    public IServiceProvider Build(ICXLServiceContainer containerBuilder)
    {
        return new CXLServiceProvider(this);
    }
}
