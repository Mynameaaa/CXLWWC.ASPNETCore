using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventSource;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.Tracing;
using System.Reflection;

namespace WWC._240711.ASPNETCore.Extensions;

public class CXLServiceProvider : IServiceProvider, IServiceScopeFactory
{
    internal ICXLServiceContainer Container { get; init; }
    private int serviceCount = 0;

    public CXLServiceProvider(ICXLServiceContainer container)
    {
        Container = container;
        Container.AddService(new CXLServiceDescriptor(typeof(IServiceProvider), typeof(CXLServiceProvider), CXLServiceLifetime.Singleton)
        {
            Instance = this,
        });
        Container.AddService(new CXLServiceDescriptor(typeof(IServiceScopeFactory), typeof(CXLServiceProvider), CXLServiceLifetime.Singleton)
        {
            Instance = this,
        });
        Container.AddService(new CXLServiceDescriptor(typeof(IServiceProviderIsService), typeof(CXLServiceProviderIsService), CXLServiceLifetime.Singleton)
        {
            Instance = new CXLServiceProviderIsService(this),
        });
        Container.AddService(new CXLServiceDescriptor(typeof(IServiceScope), typeof(CXLScoped), CXLServiceLifetime.Singleton)
        {
            Instance = new CXLScoped(this),
        });
    }

    public object? GetService(Type serviceType)
    {
        if (serviceType.Name.Contains("ISwaggerProvider"))
        {

        }

        // 查找匹配的服务描述符，支持泛型类型定义匹配
        var serviceDescriptor = Container.FirstOrDefault(p =>
            (p.InterfacesType.IsGenericTypeDefinition &&
             serviceType.IsGenericType &&
             p.InterfacesType == serviceType.GetGenericTypeDefinition()) ||
            serviceType == p.InterfacesType);

        if (serviceDescriptor == null)
        {
            if (serviceType.Name == "IDebugger")
            {
                return IDebuggerType(serviceType);
            }

            if (serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var itemType = serviceType.GetGenericArguments()[0];
                var serviceDescriptors = Container.Where(p => itemType == p.InterfacesType);

                if (serviceDescriptors == null || serviceDescriptors.Count() <= 0)
                {
                    //throw new InvalidOperationException($"未在容器中注入服务 {itemType}");
                    return null;
                }

                var implementationTypes = serviceDescriptors.Select(p => p.ImplementationType).ToList();

                return CreateInstanceByCtor(this, serviceDescriptors.ToList(), implementationTypes, serviceType);
            }
            return null;
            //throw new InvalidOperationException($"未在容器中注入服务 {serviceType}");
        }

        switch (serviceDescriptor.Lifetime)
        {
            case CXLServiceLifetime.Transient:
                if (serviceDescriptor.InstanceServiceFactory != null)
                    return serviceDescriptor.InstanceServiceFactory.Invoke(this);
                return CreateInstanceByCtor(this, serviceDescriptor, serviceDescriptor.ImplementationType, serviceType);

            case CXLServiceLifetime.Scoped:
            case CXLServiceLifetime.Singleton:
                if (serviceDescriptor.Instance != null)
                    return serviceDescriptor.Instance;
                if (serviceDescriptor.InstanceServiceFactory != null)
                    return serviceDescriptor.InstanceServiceFactory.Invoke(this);
                break;

            default:
                throw new InvalidOperationException("未知的生命周期类型");
        }

        // 如果是泛型类型且有未填充的泛型参数，则使用 MakeGenericType 生成具体类型
        var implementationType = serviceDescriptor.ImplementationType;
        if (implementationType.IsGenericTypeDefinition && serviceType.IsGenericType)
        {
            // 生成具体的泛型类型
            implementationType = implementationType.MakeGenericType(serviceType.GetGenericArguments());
        }

        var instance = CreateInstanceByCtor(this, serviceDescriptor, implementationType, serviceType);

        // 如果是非 Transient 并且不是泛型定义，则缓存实例
        if (serviceDescriptor.Lifetime != CXLServiceLifetime.Transient && !serviceDescriptor.InterfacesType.IsGenericTypeDefinition)
        {
            serviceDescriptor.Instance = instance;
        }
        serviceCount++;
        Console.WriteLine("构造数量：" + serviceCount);
        return instance;
    }

    private object CreateInstanceByCtor(IServiceProvider provider, CXLServiceDescriptor descriptor, Type implementationType, Type serviceType)
    {
        if (descriptor.Instance != null)
        {
            return descriptor.Instance;
        }

        if (descriptor.InstanceServiceFactory != default)
        {
            return descriptor.InstanceServiceFactory.Invoke(this);
        }

        if (implementationType == null) throw new Exception("在容器中需要查找的类型不能为空");

        if (implementationType.ContainsGenericParameters)
        {
            implementationType = implementationType.GetGenericTypeDefinition().MakeGenericType(serviceType.GetGenericArguments());
        }

        var constructors = default(ConstructorInfo[]);
        if (implementationType.IsInterface || implementationType.IsAbstract)
            throw new Exception($"{implementationType} 类型为抽象类型或接口");

        // 获取所有构造函数
        constructors = implementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (constructors.Length <= 0)
        {
            throw new InvalidOperationException("该服务不存在任何公共的构造函数");
        }

        ConstructorInfo createCtor;
        // 查找被标注了 DICtorAttribute 的构造函数
        var dicCtor = constructors.FirstOrDefault(c => c.GetCustomAttribute<DICtorAttribute>() != null || c.GetCustomAttribute<ActivatorUtilitiesConstructorAttribute>() != null);

        if (dicCtor != null)
        {
            createCtor = dicCtor;
        }
        else
        {
            var createCtors = new List<ConstructorInfo>();
            foreach (var constructor in constructors)
            {
                // 获取构造函数的参数信息
                var parameters = constructor.GetParameters();

                var hasService = ResolveService(parameters);

                //// 如果所有参数都可以解析，选择这个构造函数
                //if (hasService)
                //{
                createCtors.Add(constructor);
                //}
                //else
                //{
                //    throw new InvalidOperationException("找不到适合类型的构造函数" + implementationType.FullName);
                //}
            }

            createCtor = SelectCtor(createCtors, implementationType);
        }

        Console.WriteLine(implementationType.Name);
        var ctorParams = createCtor.GetParameters();

        var ctorParamsInstance = new List<object>();

        if (ctorParams.Length == 0)
        {
            var serviceInstance = createCtor.Invoke(null);
            return createCtor.Invoke(ctorParamsInstance.ToArray());
        }

        foreach (var paramter in ctorParams)
        {
            #region Custom
            object paramterInstance = default(object);

            #region 处理泛型

            if (paramter.ParameterType.IsGenericType)
            {
                var genericTypeDefinition = paramter.ParameterType.GetGenericTypeDefinition();

                if (genericTypeDefinition == typeof(IEnumerable<>))
                {
                    var createType = paramter.ParameterType;
                    if (createType.ContainsGenericParameters)
                    {
                        createType = createType.GetGenericTypeDefinition().MakeGenericType(serviceType.GetGenericArguments());
                    }

                    var genericArguments = createType.GetGenericArguments();
                    var itemType = genericArguments[0];

                    if (itemType.IsGenericType)
                    {
                        var ctorParamterDescriptor = this.Container.FirstOrDefault(p =>
                            p.InterfacesType.IsGenericType &&
                            p.InterfacesType.GetGenericTypeDefinition() == itemType.GetGenericTypeDefinition());
                        if (ctorParamterDescriptor == null)
                        {
                            paramterInstance = itemType.GetDefaultValue();
                        }
                        else
                        {
                            paramterInstance = this.GetService(ctorParamterDescriptor.InterfacesType);
                        }
                    }

                    #region 去除
                    //// 检查是否是 IConfigureOptions<> 或 IPostConfigureOptions<>
                    //if (IsMatchingGenericType(itemType, typeof(IConfigureOptions<>), out var innerGenericArguments) ||
                    //    IsMatchingGenericType(itemType, typeof(IPostConfigureOptions<>), out innerGenericArguments))
                    //{
                    //    var innerGenericTypeAguments = itemType.GetGenericArguments();

                    //    services = Container
                    //       .Where(p => itemType.IsAssignableFrom(p.InterfacesType) ||
                    //                   (p.InterfacesType.IsGenericType &&
                    //                    itemType.IsAssignableFrom(p.InterfacesType.GetGenericTypeDefinition())))
                    //       .ToList();
                    //}
                    //else
                    //{
                    //    services = Container
                    //       .Where(p => itemType.IsAssignableFrom(p.InterfacesType) ||
                    //                   (p.InterfacesType.IsGenericType &&
                    //                    itemType.IsAssignableFrom(p.InterfacesType.GetGenericTypeDefinition())))
                    //       .ToList();
                    //} 
                    #endregion

                    var services = this.Container.Where(p => p.InterfacesType == itemType).ToList();
                    Array array = Array.CreateInstance(itemType, services.Count);
                    for (int i = 0; i < services.Count; i++)
                    {
                        var serviceDescriptor = services[i];

                        if (serviceDescriptor.Instance != null)
                        {
                            paramterInstance = AdaptInstanceToType(serviceDescriptor.Instance, itemType);
                        }
                        else
                        {
                            var objectValue = CreateInstanceByCtor(this, serviceDescriptor, serviceDescriptor.ImplementationType, createType);
                            paramterInstance = AdaptInstanceToType(objectValue, itemType);
                        }
                        //Microsoft.Extensions.Logging.Configuration.LoggingConfiguration;
                        //System.Collections.Generic.IEnumerable<Microsoft.Extensions.Logging.Configuration.LoggingConfiguration>

                        array.SetValue(paramterInstance, i);
                    }

                    paramterInstance = array;
                }
                else
                {
                    // 尝试从容器中查找泛型类型定义
                    var ctorParamterDescriptor = Container.FirstOrDefault(p =>
                        p.InterfacesType.IsGenericType &&
                        p.InterfacesType.GetGenericTypeDefinition() == genericTypeDefinition);

                    if (!paramter.ParameterType.ContainsGenericParameters)
                    {
                        var ctorParamterDescriptors = Container.Where(p =>
                        p.InterfacesType.IsGenericType && p.InterfacesType.GetGenericTypeDefinition() == genericTypeDefinition);

                        foreach (var desc in ctorParamterDescriptors)
                        {
                            bool isContinue = true;
                            var types = desc.InterfacesType.GetGenericArguments().ToList();
                            foreach (var type in types)
                            {
                                if (paramter.ParameterType.GetGenericArguments()[types.IndexOf(type)] == type)
                                {
                                    isContinue = false;
                                }
                                else
                                {
                                    isContinue = true;
                                }
                            }
                            if (isContinue)
                            {
                                continue;
                            }
                            ctorParamterDescriptor = desc;
                        }
                    }

                    if (ctorParamterDescriptor.Instance != null)
                    {
                        paramterInstance = ctorParamterDescriptor.Instance;
                        ctorParamsInstance.Add(paramterInstance!);
                        continue;
                    }

                    var customAttribute = paramter.GetCustomAttribute<CXLInstanceDefaultValueAttribute>();

                    //处理通过特性注入的服务
                    if ((!customAttribute?.UseContainerService) ?? false)
                    {
                        paramterInstance = GetAttributeDefaultValue(paramter, customAttribute);
                    }
                    else
                    {
                        if (ctorParamterDescriptor == null) throw new Exception($"无法从容器中获取 {paramter} 类型的服务，并且未在参数特性中设置默认值");

                        if (ctorParamterDescriptor.Instance != null)
                        {
                            paramterInstance = ctorParamterDescriptor.Instance;
                        }

                        if (ctorParamterDescriptor.InstanceServiceFactory != default)
                        {
                            paramterInstance = ctorParamterDescriptor.InstanceServiceFactory.Invoke(this);
                        }

                        if (paramterInstance == default)
                        {
                            var createType = paramter.ParameterType;
                            if (createType.ContainsGenericParameters)
                            {
                                createType = createType.GetGenericTypeDefinition().MakeGenericType(serviceType.GetGenericArguments());
                            }

                            var genericArguments = createType.GetGenericArguments();

                            // 如果找到，创建泛型类型实例
                            var genericServiceType = ctorParamterDescriptor.ImplementationType.MakeGenericType(genericArguments);
                            paramterInstance = CreateInstanceByCtor(provider, ctorParamterDescriptor, ctorParamterDescriptor.ImplementationType, createType);
                        }
                    }

                }
            }

            #endregion

            if (paramterInstance == default(object))
            {

                var ctorParamterDescriptor = Container.FirstOrDefault(p => p.InterfacesType == paramter.ParameterType);
                var customAttribute = paramter.GetCustomAttribute<CXLInstanceDefaultValueAttribute>();

                //处理通过特性注入的服务
                if ((!customAttribute?.UseContainerService) ?? false)
                {
                    paramterInstance = GetAttributeDefaultValue(paramter, customAttribute);
                }
                else
                {
                    if (ctorParamterDescriptor == null)
                    {
                        if (paramter.HasDefaultValue)
                        {
                            paramterInstance = paramter.DefaultValue;
                        }
                        else
                        {
                            var va = this.Container.FirstOrDefault(p => p.ImplementationType != null && p.ImplementationType.Name.Contains("IStringLocalizerFactory"));
                            throw new Exception($"无法从容器中获取 {paramter} 类型的服务，并且未在参数特性中设置默认值");
                        }
                    }

                    if (ctorParamterDescriptor?.Instance != null)
                    {
                        paramterInstance = ctorParamterDescriptor.Instance;
                    }
                    else if (ctorParamterDescriptor?.InstanceServiceFactory != default)
                    {
                        paramterInstance = ctorParamterDescriptor.InstanceServiceFactory.Invoke(this);
                    }
                    else if (ctorParamterDescriptor?.ImplementationType != null)
                    {
                        paramterInstance = CreateInstanceByCtor(provider, ctorParamterDescriptor, ctorParamterDescriptor.ImplementationType, serviceType);
                    }
                }
            }

            ctorParamsInstance.Add(paramterInstance!);
            #endregion
        }

        var instance = createCtor.Invoke(ctorParamsInstance.ToArray());
        return instance;
    }

    private Array CreateInstanceByCtor(IServiceProvider provider, List<CXLServiceDescriptor> descriptors, List<Type> implementationTypes, Type serviceType)
    {
        var realType = serviceType.GetGenericArguments()[0];
        Array resultArray = Array.CreateInstance(realType, descriptors.Count);
        int resultI = 0;

        foreach (var descriptor in descriptors)
        {
            var implementationType = implementationTypes[descriptors.IndexOf(descriptor)];
            if (descriptor.Instance != null)
            {
                resultArray.SetValue(descriptor.Instance, resultI);
            }

            if (descriptor.InstanceServiceFactory != default)
            {
                resultArray.SetValue(descriptor.InstanceServiceFactory.Invoke(this), resultI);
            }

            if (implementationType == null) throw new Exception("在容器中需要查找的类型不能为空");

            if (implementationType.ContainsGenericParameters)
            {
                implementationType = implementationType.GetGenericTypeDefinition().MakeGenericType(serviceType.GetGenericArguments());
            }

            var constructors = default(ConstructorInfo[]);
            if (implementationType.IsInterface || implementationType.IsAbstract)
                throw new Exception($"{implementationType} 类型为抽象类型或接口");

            // 获取所有构造函数
            constructors = implementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (constructors.Length <= 0)
            {
                throw new InvalidOperationException("该服务不存在任何公共的构造函数");
            }

            ConstructorInfo createCtor;
            // 查找被标注了 DICtorAttribute 的构造函数
            var dicCtor = constructors.FirstOrDefault(c => c.GetCustomAttribute<DICtorAttribute>() != null || c.GetCustomAttribute<ActivatorUtilitiesConstructorAttribute>() != null);

            if (dicCtor != null)
            {
                createCtor = dicCtor;
            }
            else
            {
                var createCtors = new List<ConstructorInfo>();
                foreach (var constructor in constructors)
                {
                    // 获取构造函数的参数信息
                    var parameters = constructor.GetParameters();

                    var hasService = ResolveService(parameters);

                    // 如果所有参数都可以解析，选择这个构造函数
                    if (hasService)
                    {
                        createCtors.Add(constructor);
                    }
                    else
                    {
                        throw new InvalidOperationException("找不到适合类型的构造函数" + implementationType.FullName);
                    }
                }

                createCtor = SelectCtor(createCtors, implementationType);
            }

            Console.WriteLine(implementationType.Name);
            var ctorParams = createCtor.GetParameters();

            var ctorParamsInstance = new List<object>();

            if (ctorParams.Length == 0)
            {
                var serviceInstance = createCtor.Invoke(null);
                resultArray.SetValue(createCtor.Invoke(ctorParamsInstance.ToArray()), resultI);
            }

            foreach (var paramter in ctorParams)
            {
                #region Custom
                object paramterInstance = default(object);

                #region 处理泛型

                if (paramter.ParameterType.IsGenericType)
                {
                    var createType = paramter.ParameterType;
                    if (createType.ContainsGenericParameters)
                    {
                        createType = createType.GetGenericTypeDefinition().MakeGenericType(serviceType.GetGenericArguments());
                    }

                    var genericTypeDefinition = createType.GetGenericTypeDefinition();
                    var genericArguments = createType.GetGenericArguments();

                    if (genericTypeDefinition == typeof(IEnumerable<>))
                    {
                        var itemType = genericArguments[0];

                        if (itemType.IsGenericType)
                        {
                            var ctorParamterDescriptor = this.Container.FirstOrDefault(p =>
                                p.InterfacesType.IsGenericType &&
                                p.InterfacesType.GetGenericTypeDefinition() == itemType.GetGenericTypeDefinition());
                            if (ctorParamterDescriptor == null)
                            {
                                paramterInstance = itemType.GetDefaultValue();
                            }
                            else
                            {
                                paramterInstance = this.GetService(ctorParamterDescriptor.InterfacesType);
                            }
                        }

                        var services = this.Container.Where(p => p.InterfacesType == itemType).ToList();
                        Array array = Array.CreateInstance(itemType, services.Count);
                        for (int i = 0; i < services.Count; i++)
                        {
                            var serviceDescriptor = services[i];

                            if (serviceDescriptor.Instance != null)
                            {
                                paramterInstance = AdaptInstanceToType(serviceDescriptor.Instance, itemType);
                            }
                            else
                            {
                                var objectValue = CreateInstanceByCtor(this, serviceDescriptor, serviceDescriptor.ImplementationType, createType);
                                paramterInstance = AdaptInstanceToType(objectValue, itemType);
                            }
                            //Microsoft.Extensions.Logging.Configuration.LoggingConfiguration;
                            //System.Collections.Generic.IEnumerable<Microsoft.Extensions.Logging.Configuration.LoggingConfiguration>

                            array.SetValue(paramterInstance, i);
                        }

                        paramterInstance = array;
                    }
                    else
                    {
                        // 尝试从容器中查找泛型类型定义
                        var ctorParamterDescriptor = Container.FirstOrDefault(p =>
                            p.InterfacesType.IsGenericType &&
                            p.InterfacesType.GetGenericTypeDefinition() == genericTypeDefinition);

                        if (ctorParamterDescriptor.Instance != null)
                        {
                            paramterInstance = ctorParamterDescriptor.Instance;
                        }

                        var customAttribute = paramter.GetCustomAttribute<CXLInstanceDefaultValueAttribute>();

                        //处理通过特性注入的服务
                        if ((!customAttribute?.UseContainerService) ?? false)
                        {
                            paramterInstance = GetAttributeDefaultValue(paramter, customAttribute);
                        }
                        else
                        {
                            if (ctorParamterDescriptor == null) throw new Exception($"无法从容器中获取 {paramter} 类型的服务，并且未在参数特性中设置默认值");

                            // 如果找到，创建泛型类型实例
                            var genericServiceType = ctorParamterDescriptor.ImplementationType.MakeGenericType(genericArguments);
                            paramterInstance = CreateInstanceByCtor(provider, ctorParamterDescriptor, ctorParamterDescriptor.ImplementationType, createType);
                        }

                    }
                }

                #endregion

                if (paramterInstance == default(object))
                {

                    var ctorParamterDescriptor = Container.FirstOrDefault(p => p.InterfacesType == paramter.ParameterType);
                    var customAttribute = paramter.GetCustomAttribute<CXLInstanceDefaultValueAttribute>();

                    //处理通过特性注入的服务
                    if ((!customAttribute?.UseContainerService) ?? false)
                    {
                        paramterInstance = GetAttributeDefaultValue(paramter, customAttribute);
                    }
                    else
                    {
                        if (ctorParamterDescriptor == null)
                        {
                            if (paramter.HasDefaultValue)
                            {
                                paramterInstance = paramter.DefaultValue;
                            }
                            else
                            {
                                throw new Exception($"无法从容器中获取 {paramter} 类型的服务，并且未在参数特性中设置默认值");
                            }
                        }

                        if (ctorParamterDescriptor?.Instance != null)
                        {
                            paramterInstance = ctorParamterDescriptor.Instance;
                        }
                        else if (ctorParamterDescriptor?.InstanceServiceFactory != null)
                        {
                            paramterInstance = ctorParamterDescriptor.InstanceServiceFactory.Invoke(this);
                        }
                        else if (ctorParamterDescriptor?.ImplementationType != null)
                        {
                            paramterInstance = CreateInstanceByCtor(provider, ctorParamterDescriptor, ctorParamterDescriptor.ImplementationType, serviceType);
                        }
                    }
                }

                ctorParamsInstance.Add(paramterInstance!);
                #endregion
            }

            var instance = createCtor.Invoke(ctorParamsInstance.ToArray());
            resultArray.SetValue(instance, resultI);
            resultI++;
        }
        return resultArray;
    }

    private object IDebuggerType(Type idbuggerType)
    {
        var instanceTypeAssembly = idbuggerType.Assembly;
        var allName = instanceTypeAssembly.GetTypes().Where(p => p.Name == "DebuggerWrapper").Select(p => p.FullName).FirstOrDefault();
        var type = instanceTypeAssembly.GetType(allName);
        // 获取internal类型的字段
        var field = type.GetField("Instance", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

        return field.GetValue(null);
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

    private object AdaptInstanceToType(object instance, Type targetType)
    {
        if (targetType == typeof(ILoggerProvider) && instance is EventSource eventSource)
        {
            return new EventSourceLoggerProvider((LoggingEventSource)eventSource);
        }
        return instance;
    }

    public bool ResolveService(ParameterInfo[] parameterTypes)
    {
        bool canCreate = true;

        foreach (var paramter in parameterTypes)
        {
            if (paramter.ParameterType.IsArray)
            {
                canCreate = false;
                break;
            }

            if (this.Container.FirstOrDefault(p => p.InterfacesType == paramter.ParameterType) == null)
            {
                if (paramter.DefaultValue == DBNull.Value)
                {
                    //可空类型
                }
            }
            else
            {
                if (paramter.ParameterType.IsGenericType)
                {
                    if (paramter.ParameterType == typeof(IEnumerable<>))
                    {
                        //泛型类型
                        if (paramter.ParameterType.ContainsGenericParameters)
                        {
                            canCreate = false;
                            break;
                        }

                        var genericArguments = paramter.ParameterType.GetGenericArguments();
                        //泛型集合类型
                        var itemType = genericArguments[0];
                        canCreate = Container
                            .Where(p => itemType.IsAssignableFrom(p.InterfacesType) ||
                                        (p.InterfacesType.IsGenericType &&
                                         itemType.IsAssignableFrom(p.InterfacesType.GetGenericTypeDefinition()))).Any();
                    }
                    else
                    {
                        //泛型类型
                        if (paramter.ParameterType.ContainsGenericParameters)
                        {
                            canCreate = false;
                            break;
                        }

                        var genericTypeDefinition = paramter.ParameterType.GetGenericTypeDefinition();
                        canCreate = Container.Any(p =>
                            p.InterfacesType.IsGenericType &&
                            p.InterfacesType.GetGenericTypeDefinition() == genericTypeDefinition);
                    }
                }
            }
        }

        return canCreate;
    }

    public ConstructorInfo SelectCtor(List<ConstructorInfo> constructorInfos, Type implementationType)
    {
        // 构造函数按参数数量降序排列
        var sortedConstructors = constructorInfos.OrderByDescending(c => c.GetParameters().Length);
        ConstructorInfo bestConstructor = null;
        int bestScore = int.MinValue;

        foreach (var constructorInfo in sortedConstructors)
        {
            int score = 0;
            bool canResolveAllParameters = true;

            foreach (var parameter in constructorInfo.GetParameters())
            {
                Type parameterType = parameter.ParameterType;

                // 处理 IEnumerable<T> 类型
                if (parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    // 检查容器中是否有 T 类型的服务
                    Type serviceType = parameterType.GetGenericArguments()[0];
                    if (this.Container.Any(p => p.InterfacesType == serviceType))
                    {
                        score += 10; // 如果容器中有 T 类型的服务，增加权重
                    }
                    else
                    {
                        // 即使容器中没有，也可以解析为空集合
                        score += 5; // 赋予较低权重
                    }
                }
                // 处理普通泛型类型
                else if (parameterType.IsGenericType)
                {
                    Type genericTypeDefinition = parameterType.GetGenericTypeDefinition();
                    if (this.Container.Any(p => p.InterfacesType.IsGenericType && p.InterfacesType.GetGenericTypeDefinition() == genericTypeDefinition))
                    {
                        score += 10; // 泛型服务可解析，增加权重
                    }
                    else if (parameter.HasDefaultValue || Nullable.GetUnderlyingType(parameter.ParameterType) != null)
                    {
                        score += 2; // 有默认值或可空类型，赋予较低权重
                    }
                    else
                    {
                        canResolveAllParameters = false;
                        break; // 无法解析泛型服务，跳过这个构造函数
                    }
                }
                // 处理普通类型
                else
                {
                    if (this.Container.Any(p => p.InterfacesType == parameterType))
                    {
                        score += 10; // 普通类型可解析，增加权重
                    }
                    else if (parameter.HasDefaultValue || Nullable.GetUnderlyingType(parameter.ParameterType) != null)
                    {
                        score += 2; // 有默认值或可空类型，赋予较低权重
                    }
                    else
                    {
                        canResolveAllParameters = false;
                        break; // 无法解析普通类型，跳过这个构造函数
                    }
                }
            }

            // 如果所有参数都可解析，且得分比当前最佳的高，则选择此构造函数
            if (canResolveAllParameters && score > bestScore)
            {
                bestConstructor = constructorInfo;
                bestScore = score;
            }
        }

        if (bestConstructor != null)
        {
            return bestConstructor;
        }

        // 如果没有找到合适的构造函数，抛出异常
        throw new Exception($"The service {implementationType.Name} does not have a suitable constructor.");
    }

    //public ConstructorInfo SelectCtor(List<ConstructorInfo> constructorInfos, Type implementationType)
    //{
    //    ConstructorInfo bestConstructor = null;
    //    int highestResolvedParamsCount = -1;
    //    int highestConstructorLevel = -1;

    //    foreach (var constructorInfo in constructorInfos)
    //    {
    //        int resolvedParamsCount = 0; // 已经成功解析的参数数量
    //        int constructorLevel = 0;    // 构造函数的级别
    //        bool hasUnresolvableParams = false;

    //        foreach (var parameter in constructorInfo.GetParameters())
    //        {
    //            // 根据参数类型增加优先级
    //            if (Nullable.GetUnderlyingType(parameter.ParameterType) != null)
    //            {
    //                constructorLevel += CtorLevel.Nullable;
    //            }
    //            else if (parameter.ParameterType.IsGenericType)
    //            {
    //                constructorLevel += CtorLevel.Generic;
    //            }
    //            else if (parameter.ParameterType.IsInterface || (parameter.ParameterType.IsClass && parameter.ParameterType.IsAbstract))
    //            {
    //                bool isCollectionType = parameter.ParameterType.IsArray ||
    //                    (parameter.ParameterType.IsGenericType &&
    //                     typeof(IEnumerable<>).IsAssignableFrom(parameter.ParameterType.GetGenericTypeDefinition()));

    //                if (!isCollectionType)
    //                {
    //                    constructorLevel += CtorLevel.InterfaceType;
    //                }
    //            }
    //            else
    //            {
    //                constructorLevel += CtorLevel.Default;
    //            }

    //            // 检查容器是否能够解析此参数类型
    //            if (this.Container.Any(p => p.InterfacesType == parameter.ParameterType))
    //            {
    //                resolvedParamsCount++;
    //            }
    //            else
    //            {
    //                // 参数无法解析，不要立即放弃这个构造函数，但降低优先级
    //                hasUnresolvableParams = true;
    //                //constructorLevel -= CtorLevel.InterfaceType;
    //            }
    //        }

    //        // 优先选择解析成功参数最多且优先级更高的构造函数
    //        if (resolvedParamsCount > highestResolvedParamsCount ||
    //            (resolvedParamsCount == highestResolvedParamsCount && constructorLevel > highestConstructorLevel))
    //        {
    //            highestResolvedParamsCount = resolvedParamsCount;
    //            highestConstructorLevel = constructorLevel;
    //            bestConstructor = constructorInfo;
    //        }
    //    }

    //    // 如果没有找到合适的构造函数，抛出异常
    //    if (bestConstructor == null)
    //    {
    //        throw new Exception($"The service {implementationType.Name} does not have a suitable constructor.");
    //    }

    //    return bestConstructor;
    //}


    //public ConstructorInfo SelectCtor(List<ConstructorInfo> constructorInfos, Type implementationType)
    //{
    //    //var constructorInfoGroup = constructorInfos.GroupBy(p => p.GetParameters().Length).OrderByDescending(p => p.Key).FirstOrDefault();

    //    //if (constructorInfoGroup == null)
    //    //{
    //    //    throw new Exception($"该服务 {serviceType} 不存在任何合适的构造函数");
    //    //}

    //    //if (constructorInfoGroup.Count() == 1)
    //    //{
    //    //    return constructorInfoGroup.First();
    //    //}
    //    ConstructorInfo constructor = null;

    //    int currentLevel = 0;
    //    foreach (var constructorInfo in constructorInfos)
    //    {
    //        if (constructor == null)
    //        {
    //            constructor = constructorInfo;
    //        }

    //        int thisConstructorLevel = 0;
    //        bool hasNoCreateService = false;
    //        foreach (var parameter in constructorInfo.GetParameters())
    //        {
    //            if (Nullable.GetUnderlyingType(parameter.ParameterType) != null)
    //            {
    //                thisConstructorLevel += CtorLevel.Nullable;
    //            }
    //            else if (parameter.ParameterType.IsGenericType)
    //            {
    //                thisConstructorLevel += CtorLevel.Generic;
    //            }
    //            else if (parameter.ParameterType.IsInterface || (parameter.ParameterType.IsClass && parameter.ParameterType.IsAbstract))
    //            {
    //                bool isCollectionType = parameter.ParameterType.IsArray ||
    //                        (parameter.ParameterType.IsGenericType &&
    //                         typeof(IEnumerable<>).IsAssignableFrom(parameter.ParameterType.GetGenericTypeDefinition()));
    //                if (!isCollectionType)
    //                {
    //                    thisConstructorLevel += CtorLevel.InterfaceType;
    //                }
    //            }
    //            else
    //            {
    //                thisConstructorLevel += CtorLevel.Default;
    //            }

    //            if (!parameter.ParameterType.IsGenericType && !this.Container.Any(p => p.InterfacesType == parameter.ParameterType))
    //            {
    //                hasNoCreateService = true;
    //                thisConstructorLevel = 1;
    //            }
    //        }

    //        if (thisConstructorLevel > currentLevel)
    //        {
    //            currentLevel = thisConstructorLevel;
    //            constructor = constructorInfo;
    //        }
    //    }

    //    return constructor;
    //}

    //private object? HandleGenericType(ParameterInfo parameter)
    //{
    //    if (genericTypeDefinition != typeof(IEnumerable<>))
    //    {
    //        var innerItemType = genericArguments[0];

    //        // 继续处理更深层的泛型
    //        if (innerItemType.IsGenericType)
    //        {
    //            var deeperGenericTypeDefinition = innerItemType.GetGenericTypeDefinition();
    //            var deeperGenericArguments = innerItemType.GetGenericArguments();
    //            innerItemType = HandleGenericType(innerItemType, deeperGenericTypeDefinition, deeperGenericArguments);
    //        }

    //        // 可以根据需求继续处理内层泛型
    //    }
    //    else if (genericTypeDefinition == typeof(IEnumerable<>))
    //    {

    //    }

    //    return innerType;
    //}

    private Type HandleInnerGenericType(Type innerType, Type genericTypeDefinition, Type[] genericArguments)
    {
        if (genericTypeDefinition != typeof(IEnumerable<>))
        {
            var innerItemType = genericArguments[0];

            // 继续处理更深层的泛型
            if (innerItemType.IsGenericType)
            {
                var deeperGenericTypeDefinition = innerItemType.GetGenericTypeDefinition();
                var deeperGenericArguments = innerItemType.GetGenericArguments();
                innerItemType = HandleInnerGenericType(innerItemType, deeperGenericTypeDefinition, deeperGenericArguments);
            }

            // 可以根据需求继续处理内层泛型
        }
        else if (genericTypeDefinition == typeof(IEnumerable<>))
        {

        }

        return innerType;
    }

    public static bool IsMatchingGenericType(Type typeToCheck, Type targetGenericType, out Type[] genericArguments)
    {
        genericArguments = Array.Empty<Type>();

        if (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == targetGenericType)
        {
            genericArguments = typeToCheck.GetGenericArguments();
            return true;
        }

        return false;
    }

    //public bool ResolveService(ICXLServiceContainerBuilder services, Type parameterType)
    //{
    //    // 处理可空类型，例如 int? 或 Nullable<int>
    //    if (IsNullableType(parameterType))
    //    {
    //        // 获取底层非可空的类型
    //        var underlyingType = Nullable.GetUnderlyingType(parameterType);
    //        if (underlyingType != null)
    //        {
    //            return IsServiceRegistered(services, underlyingType);
    //        }
    //    }

    //    // 处理泛型类型，例如 IEnumerable<T>
    //    if (parameterType.IsGenericType)
    //    {
    //        // 例如 IList<T>, ICollection<T> 等集合类型
    //        if (parameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
    //            || parameterType.GetGenericTypeDefinition() == typeof(IList<>)
    //            || parameterType.GetGenericTypeDefinition() == typeof(ICollection<>)
    //            || parameterType.GetGenericTypeDefinition() == typeof(IReadOnlyList<>)
    //            || parameterType.GetGenericTypeDefinition() == typeof(IReadOnlyCollection<>))
    //        {
    //            // 获取泛型参数类型
    //            var genericArgument = parameterType.GetGenericArguments()[0];
    //            var enumerableType = typeof(IEnumerable<>).MakeGenericType(genericArgument);
    //            return IsServiceRegistered(services, enumerableType);
    //        }

    //        // 处理其他泛型类型
    //        return IsServiceRegistered(services, parameterType);
    //    }

    //    // 处理普通类型
    //    return IsServiceRegistered(services, parameterType);
    //}

    // 检查类型是否为可空类型
    private bool IsNullableType(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    // 检查服务是否已注册
    private bool IsServiceRegistered(ICXLServiceContainer services, Type serviceType)
    {
        return services.Any(sd => sd.InterfacesType == serviceType);
    }

    public IServiceScope CreateScope()
    {
        return new CXLScoped(this);
    }

    #region AI

    //public object? GetService(Type serviceType)
    //{
    //    var serviceDescriptor = Container.FirstOrDefault(p => p.InterfacesType == serviceType);
    //    if (serviceDescriptor == null)
    //        throw new InvalidOperationException($"未在容器中注入服务 {serviceType}");

    //    return ResolveService(serviceDescriptor);
    //}

    //private object ResolveService(CXLServiceDescriptor serviceDescriptor)
    //{
    //    switch (serviceDescriptor.Lifetime)
    //    {
    //        case CXLServiceLifetime.Transient:
    //            return CreateInstanceByCtor(serviceDescriptor.ImplementationType);
    //        case CXLServiceLifetime.Scoped:
    //        case CXLServiceLifetime.Singleton:
    //            return ResolveScopedOrSingletonService(serviceDescriptor);
    //        default:
    //            throw new InvalidOperationException("未知的服务生命周期");
    //    }
    //}

    //private object ResolveScopedOrSingletonService(CXLServiceDescriptor serviceDescriptor)
    //{
    //    if (serviceDescriptor.InstanceServiceFactory != null)
    //        return serviceDescriptor.InstanceServiceFactory.Invoke(this);
    //    if (serviceDescriptor.Instance != null)
    //        return serviceDescriptor.Instance;

    //    serviceDescriptor.Instance = CreateInstanceByCtor(serviceDescriptor.ImplementationType);
    //    return serviceDescriptor.Instance;
    //}

    //private object CreateInstanceByCtor(Type implementationType)
    //{
    //    var constructors = implementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    //    if (constructors.Length == 0)
    //        throw new InvalidOperationException("该服务不存在任何构造函数");

    //    var constructor = constructors.OrderByDescending(c => c.GetParameters().Length).FirstOrDefault()
    //                       ?? throw new InvalidOperationException("无法找到合适的构造函数");

    //    var parameters = constructor.GetParameters();
    //    var parameterInstances = parameters.Select(p => ResolveParameter(p)).ToArray();

    //    return constructor.Invoke(parameterInstances);
    //}

    //private ConstructorInfo GetConstructor(Type implementationType)
    //{
    //    var constructors = implementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    //    var dicCtor = constructors.FirstOrDefault(c => c.GetCustomAttribute<DICtorAttribute>() != null);

    //    return dicCtor ?? constructors.OrderByDescending(c => c.GetParameters().Length).FirstOrDefault()
    //           ?? throw new InvalidOperationException("不存在符合规则的构造函数");
    //}

    //private object ResolveParameter(ParameterInfo parameter)
    //{
    //    var parameterType = parameter.ParameterType;

    //    // 处理泛型类型
    //    if (parameterType.IsGenericType)
    //    {
    //        var genericTypeDefinition = parameterType.GetGenericTypeDefinition();
    //        var genericArguments = parameterType.GetGenericArguments();

    //        if (genericTypeDefinition == typeof(IOptions<>))
    //        {
    //            var optionsType = genericArguments[0];
    //            var optionsService = Container.FirstOrDefault(p =>
    //                p.InterfacesType.IsGenericType &&
    //                p.InterfacesType.GetGenericTypeDefinition() == typeof(IOptions<>) &&
    //                p.InterfacesType.GetGenericArguments()[0] == optionsType);

    //            if (optionsService != null)
    //            {
    //                return optionsService.Instance;
    //            }
    //        }
    //        else if (genericTypeDefinition == typeof(IOptionsMonitor<>))
    //        {
    //            var optionsMonitorType = genericArguments[0];
    //            var optionsMonitorService = Container.FirstOrDefault(p =>
    //                p.InterfacesType.IsGenericType &&
    //                p.InterfacesType.GetGenericTypeDefinition() == typeof(IOptionsMonitor<>) &&
    //                p.InterfacesType.GetGenericArguments()[0] == optionsMonitorType);

    //            if (optionsMonitorService != null)
    //            {
    //                return optionsMonitorService.Instance;
    //            }
    //        }
    //        else if (genericTypeDefinition == typeof(IEnumerable<>))
    //        {
    //            var itemType = genericArguments[0];
    //            var services = Container
    //                .Where(p => itemType.IsAssignableFrom(p.InterfacesType) ||
    //                            (p.InterfacesType.IsGenericType && itemType.IsAssignableFrom(p.InterfacesType.GetGenericTypeDefinition())))
    //                .ToList();

    //            Array array = Array.CreateInstance(itemType, services.Count);
    //            for (int i = 0; i < services.Count; i++)
    //            {
    //                var serviceDescriptor = services[i];

    //                if (serviceDescriptor.Instance != null)
    //                {
    //                    if (itemType.IsInstanceOfType(serviceDescriptor.Instance))
    //                    {
    //                        array.SetValue(serviceDescriptor.Instance, i);
    //                    }
    //                    else
    //                    {
    //                        array.SetValue(AdaptInstanceToType(serviceDescriptor.Instance, itemType), i);
    //                    }
    //                }
    //                else
    //                {
    //                    var objectValue = CreateInstanceByCtor(serviceDescriptor.ImplementationType);

    //                    if (itemType.IsInstanceOfType(objectValue))
    //                    {
    //                        array.SetValue(objectValue, i);
    //                    }
    //                    else
    //                    {
    //                        array.SetValue(AdaptInstanceToType(objectValue, itemType), i);
    //                    }
    //                }
    //            }

    //            return array;
    //        }
    //        else
    //        {
    //            // 处理其他泛型类型
    //            var descriptor = Container.FirstOrDefault(p =>
    //                p.InterfacesType.IsGenericType &&
    //                p.InterfacesType.GetGenericTypeDefinition() == genericTypeDefinition);

    //            if (descriptor != null)
    //            {
    //                if (descriptor.Instance != null)
    //                {
    //                    return descriptor.Instance;
    //                }

    //                var customAttribute = parameter.GetCustomAttribute<CXLInstanceDefaultValueAttribute>();
    //                if ((customAttribute?.UseContainerService ?? false))
    //                {
    //                    return GetAttributeDefaultValue(parameter, customAttribute);
    //                }
    //                else
    //                {
    //                    var genericServiceType = descriptor.ImplementationType.MakeGenericType(genericArguments);
    //                    return CreateInstanceByCtor(genericServiceType);
    //                }
    //            }
    //        }
    //    }
    //    else
    //    {
    //        var serviceDescriptor = Container.FirstOrDefault(p => p.InterfacesType == parameterType);
    //        if (serviceDescriptor != null)
    //        {
    //            return serviceDescriptor.Instance;
    //        }
    //    }

    //    throw new InvalidOperationException($"无法从容器中获取 {parameter.ParameterType} 类型的服务");
    //}


    //private object ResolveEnumerableParameter(Type itemType)
    //{
    //    var services = Container
    //        .Where(p => itemType.IsAssignableFrom(p.InterfacesType) ||
    //                    (p.InterfacesType.IsGenericType && itemType.IsAssignableFrom(p.InterfacesType.GetGenericTypeDefinition())))
    //        .ToList();

    //    Array array = Array.CreateInstance(itemType, services.Count);
    //    for (int i = 0; i < services.Count; i++)
    //    {
    //        var serviceDescriptor = services[i];
    //        object instance = serviceDescriptor.Instance ?? CreateInstanceByCtor(serviceDescriptor.ImplementationType);
    //        array.SetValue(AdaptInstanceToType(instance, itemType), i);
    //    }

    //    return array;
    //}

    //private object ResolveGenericParameter(Type parameterType, Type[] genericArguments)
    //{
    //    var serviceDescriptor = Container.FirstOrDefault(p =>
    //        p.InterfacesType.IsGenericType &&
    //        p.InterfacesType.GetGenericTypeDefinition() == parameterType.GetGenericTypeDefinition());

    //    if (serviceDescriptor == null)
    //        throw new InvalidOperationException($"无法从容器中获取 {parameterType} 类型的服务");

    //    var implementationType = serviceDescriptor.ImplementationType.MakeGenericType(genericArguments);
    //    return CreateInstanceByCtor(implementationType);
    //}

    //private object ResolveRegularParameter(ParameterInfo parameter)
    //{
    //    var serviceDescriptor = Container.FirstOrDefault(p => p.InterfacesType == parameter.ParameterType);
    //    if (serviceDescriptor == null)
    //        throw new InvalidOperationException($"无法从容器中获取 {parameter.ParameterType} 类型的服务");

    //    return serviceDescriptor.Instance ?? CreateInstanceByCtor(serviceDescriptor.ImplementationType);
    //}

    //private object AdaptInstanceToType(object instance, Type targetType)
    //{
    //    if (targetType == typeof(ILoggerProvider) && instance is EventSource eventSource)
    //    {
    //        return new EventSourceLoggerProvider((LoggingEventSource)eventSource);
    //    }

    //    // 处理其他特殊的适配逻辑
    //    return instance;
    //}

    #endregion
}
