using System.Reflection;
using System.Text.Json;

namespace WWC._240711.ASPNETCore.Ocelot;

public static class CXLLoadAessmblyExtensions
{

    public static IServiceCollection LoadNuGetAssemblies(this IServiceCollection services)
    {
        var assemblies = GetAssembliesFromDepsJson(out var projectName);

        foreach (var assembly in assemblies.NuGetAssemblies)
        {
            var loadedAssembly = Assembly.Load(assembly);
            // 可以根据需要注册到 services 中
            // services.AddSingleton(loadedAssembly); // 示例，实际根据具体需求调整
        }

        return services;
    }

    public static IServiceCollection LoadProjectAssemblies(this IServiceCollection services)
    {
        var assemblies = GetAssembliesFromDepsJson(out var projectName);

        foreach (var assembly in assemblies.ProjectAssemblies)
        {
            var loadedAssembly = Assembly.Load(assembly);
            // 可以根据需要注册到 services 中
            // services.AddSingleton(loadedAssembly); // 示例，实际根据具体需求调整
        }

        return services;
    }

    private static AssemblyInfo GetAssembliesFromDepsJson(out string projectName)
    {
        var depsJsonFilePath = Directory.GetFiles(AppContext.BaseDirectory, "*.deps.json").FirstOrDefault();
        if (depsJsonFilePath == null)
        {
            throw new FileNotFoundException("No .deps.json file found in the base directory.");
        }

        var jsonContent = File.ReadAllText(depsJsonFilePath);
        var depsJson = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent);
        if (depsJson == null || !depsJson.ContainsKey("targets"))
        {
            throw new InvalidOperationException("Invalid .deps.json file format.");
        }

        var targets = depsJson["targets"] as JsonElement?;
        if (targets == null)
        {
            throw new InvalidOperationException("Could not find 'targets' node in .deps.json file.");
        }

        var assembliesInfo = new AssemblyInfo();
        projectName = Assembly.GetExecutingAssembly().GetName().Name;

        foreach (var target in targets.Value.EnumerateObject())
        {
            foreach (var dependency in target.Value.EnumerateObject())
            {
                var assemblyName = dependency.Name;
                if (IsProjectAssembly(assemblyName, projectName))
                {
                    assembliesInfo.ProjectAssemblies.Add(assemblyName);
                }
                else
                {
                    assembliesInfo.NuGetAssemblies.Add(assemblyName);
                }
            }
        }

        return assembliesInfo;
    }

    private static bool IsProjectAssembly(string assemblyName, string projectName)
    {
        return assemblyName.StartsWith(projectName, StringComparison.OrdinalIgnoreCase);
    }
}

public class AssemblyInfo
{
    public List<string> ProjectAssemblies { get; set; } = new List<string>();
    public List<string> NuGetAssemblies { get; set; } = new List<string>();
}
