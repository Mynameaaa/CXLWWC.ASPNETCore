using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Extensions.Configuration.Custom;
using WWC._240711.ASPNETCore.Infrastructure;

namespace WWC._240711.ASPNETCore.Extensions
{
    public static class CXLHttpClientExtenions
    {

        public static IHttpClientBuilder AddCXLByConfigureHttpClient(this IServiceCollection services)
        {
            return services.AddHttpClient<ConfigurationHttpClient>(client =>
            {
                client.BaseAddress = new Uri(Appsettings.app("ConfigureHttpClientBaseUrl"));
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
                client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "HttpClientFactory-Sample-Typed");
            });
        }

        /// <summary>
        /// 注册配置中的 HttpClient
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IServiceCollection AddCXLHttpClient(this IServiceCollection services, string sectionKey = null)
        {
            var configures = Appsettings.app<List<NamedHttpClientOptions>?>(sectionKey ?? "NamedHttpClientOptions");

            if (configures == null)
            {
                Console.WriteLine("http 客户端服务器未开启");
                return services;
            }

            foreach (var configure in configures)
            {
                if (string.IsNullOrWhiteSpace(configure.Name) || string.IsNullOrWhiteSpace(configure.Name))
                    throw new Exception("HttpClient 配置的 Name 或者 BaseUrl 不能为空");

                services.AddHttpClient<HttpClient>(configure.Name, client =>
                {
                    client.BaseAddress = new Uri(configure.BaseUrl);
                    if (configure.DefaultHeaders != default)
                    {
                        foreach (var header in configure.DefaultHeaders)
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                    }
                });
            }

            return services;
        }

    }
}
