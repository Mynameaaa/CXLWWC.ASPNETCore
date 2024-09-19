using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Extensions.Configuration.Custom;

namespace WWC._240711.ASPNETCore.Extensions.Http.Custom
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

    }
}
