using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WWC._240711.ASPNETCore.Extensions;

public class CXLServiceSubProvider : ICXLServiceSubProvider, IServiceProvider
{
    private Dictionary<Type, object> _scopedInstances = new Dictionary<Type, object>();

    private readonly IList<CXLServiceDescriptor> _serviceCollection = new List<CXLServiceDescriptor>();

    private bool _isDisposed = false;

    public CXLServiceProvider ServiceProvider { get; }

    public CXLServiceSubProvider(CXLServiceProvider serviceProvider)
    {
        foreach (var item in serviceProvider.Container)
        {
            _serviceCollection.Add(item);
        }
        this.ServiceProvider = serviceProvider;
    }

    private CXLServiceDescriptor GetServiceDescriptor(Type serviceType)
    {
        // 从 IServiceProvider 或者 IConfigurationServiceProvider 获取描述符
        var serviceContainerBuilder = ServiceProvider.GetService<ICXLServiceContainer>();
        var serviceDescriptor = serviceContainerBuilder?.FirstOrDefault(d => d.InterfacesType == serviceType);
        if (serviceDescriptor == null)
            throw new Exception($"未在容器中找到 {serviceType} 类型的服务");

        return new CXLServiceDescriptor(serviceDescriptor.InterfacesType, serviceDescriptor.ImplementationType, (CXLServiceLifetime)serviceDescriptor.Lifetime)
        {
            InstanceServiceFactory = serviceDescriptor.InstanceServiceFactory ?? default(Func<IServiceProvider, object>),
            Instance = default(object)
        };
    }

    public object GetService(Type interfaceType)
    {
        Console.WriteLine(interfaceType.Name);
        var serviceDescriptor = GetServiceDescriptor(interfaceType);

        switch (serviceDescriptor.Lifetime)
        {
            case CXLServiceLifetime.Singleton:
                return ServiceProvider.GetService(serviceDescriptor.InterfacesType);
            case CXLServiceLifetime.Scoped:
                if (!_scopedInstances.ContainsKey(serviceDescriptor.InterfacesType))
                {
                    _scopedInstances[serviceDescriptor.InterfacesType] = GetServiceInstance(interfaceType);
                }
                return _scopedInstances[serviceDescriptor.InterfacesType];
            case CXLServiceLifetime.Transient:
                return ServiceProvider.GetService(serviceDescriptor.InterfacesType);
            default:
                break;
        }
        throw new NotImplementedException("不正确的生命周期");
    }

    private object GetServiceInstance(Type interfaceType)
    {
        var serviceDescriptor = _serviceCollection.FirstOrDefault(p => p.InterfacesType == interfaceType);
        if (serviceDescriptor == null) throw new InvalidOperationException($"未在容器中注入服务 {interfaceType}");

        return CreateInstanceByCtor(this, serviceDescriptor);
    }

    private object CreateInstanceByCtor(ICXLServiceSubProvider provider, CXLServiceDescriptor serviceDescriptor)
    {
        if (_scopedInstances.ContainsKey(serviceDescriptor.InterfacesType))
            return _scopedInstances[serviceDescriptor.InterfacesType];

        var type = serviceDescriptor.ImplementationType;
        var constructors = default(ConstructorInfo[]);

        if (type == null)
        {
            if (serviceDescriptor.InstanceServiceFactory == default(Func<IServiceProvider, object>))
                throw new Exception($"{serviceDescriptor} 类型既没有具体实现也不存在工厂");

            return serviceDescriptor.InstanceServiceFactory(this);
        }
        else
        {
            if (type.IsInterface || type.IsAbstract)
                throw new Exception($"{serviceDescriptor} 类型为抽象类型或接口");

            // 获取所有构造函数
            constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        if (constructors.Length <= 0)
        {
            throw new InvalidOperationException("该服务不存在任何公共的构造函数");
        }

        ConstructorInfo createCtor;
        // 查找被标注了 DICtorAttribute 的构造函数
        var dicCtor = constructors.FirstOrDefault(c => c.GetCustomAttribute<DICtorAttribute>() != null);

        if (dicCtor != null)
        {
            createCtor = dicCtor;
        }
        else
        {
            createCtor = constructors.OrderByDescending(c => c.GetParameters().Length).FirstOrDefault() ?? throw new Exception("不存在符合规则的构造函数");
        }

        var ctorParams = createCtor.GetParameters();
        var ctorParamsInstance = new List<object>();

        if (ctorParams.Length == 0)
        {
            var serviceInstance = createCtor.Invoke(null);
            serviceDescriptor.Instance = createCtor.Invoke(ctorParamsInstance.ToArray());
            return serviceDescriptor.Instance;
        }
        else
        {
            foreach (var paramter in ctorParams)
            {
                var ctorParamterDescriptor = _serviceCollection.FirstOrDefault(p => p.InterfacesType == paramter.ParameterType);
                var customAttribute = paramter.GetCustomAttribute<CXLInstanceDefaultValueAttribute>();
                object paramterInstance;

                //处理通过特性注入的服务
                if ((!customAttribute?.UseContainerService) ?? false)
                {
                    paramterInstance = GetAttributeDefaultValue(paramter, customAttribute);
                }
                else
                {
                    if (ctorParamterDescriptor == null)throw new Exception($"无法从容器中获取 {paramter} 类型的服务，并且未在参数特性中设置默认值");
                    paramterInstance = CreateInstanceByCtor(provider, ctorParamterDescriptor);
                }

                ctorParamsInstance.Add(paramterInstance);
            }
        }

        var instance = createCtor.Invoke(ctorParamsInstance.ToArray());
        return instance;
    }

    private object GetAttributeDefaultValue(ParameterInfo parameter, CXLInstanceDefaultValueAttribute valueAttribute)
    {
        if (valueAttribute is CXLStringInstanceDefaultValueAttribute stringDefault)
        {
            if (parameter.ParameterType != typeof(string))
                throw new Exception($"默认值类型和参数类型不匹配，默认值类型：{typeof(string)}，参数类型：{parameter.ParameterType}");
            return stringDefault.DefaultValue;
        }
        throw new Exception("出现了未知的类型");
    }

    public void Dispose()
    {
        _isDisposed = true;
        foreach (var scopedInstance in _scopedInstances.Values)
        {
            (scopedInstance as IDisposable)?.Dispose();
        }
        _scopedInstances.Clear();
    }

}
