using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WWC._240711.ASPNETCore.Extensions.Configuration.Custom;
using Microsoft.Extensions.Configuration.Json;
using WWC._240711.ASPNETCore.Extensions.Configuration.Custom.ConfigurationSource;
using System.Reflection;

namespace WWC._240711.ASPNETCore.Extensions
{
    public static class ConfigurationExtensions
    {
        public static WebApplicationBuilder InitConfiguration(this WebApplicationBuilder builder)
        {
            var appsettings = new Appsettings(builder.Configuration);
            builder.Services.AddSingleton(appsettings);
            return builder;
        }

        public static ConfigurationManager AddDefaultDeveJsonFile(this ConfigurationManager configuration)
        {
            return configuration.AddDeveJsonFile("Configuration\\Custom\\configuration.develop.json");
        }

        public static ConfigurationManager AddDeveJsonFile(this ConfigurationManager configuration, string filePath)
        {
            var basePath = AppContext.BaseDirectory;
            string jsonFilePath = Path.Combine(basePath, filePath);
            if (!File.Exists(jsonFilePath))
            {
                Console.WriteLine("【configuration.develop.json】 配置文件未能成功加载");
                return configuration;
            }

            configuration.AddJsonFile(jsonFilePath, false, true);
            return configuration;
        }

        public static ConfigurationManager AddDataBaseConfiguration(this ConfigurationManager configuration, string connectionString)
        {
            //未完成
            configuration.Sources.Add(new CustomDatabaseConfigurationSource());
            return configuration;
        }

        public static ConfigurationManager AddDefaultWebConfigFile(this ConfigurationManager configuration)
        {
            return configuration.AddWebConfigFile("Configuration\\Custom\\web.config");
        }

        /// <summary>
        /// 对 config 类型的文件提供支持
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static ConfigurationManager AddWebConfigFile(this ConfigurationManager configuration, string filePath)
        {
            var basePath = AppContext.BaseDirectory;

            string webConfigFile = Path.Combine(basePath, filePath);
            if (!File.Exists(webConfigFile))
            {
                Console.WriteLine("【web.config】 配置文件未能成功加载");
                return configuration;
            }

            configuration.Sources.Add(new CXLWebconfigConfigurationSource(webConfigFile));
            return configuration;
        }

    }
}
