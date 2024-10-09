using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using WWC._240711.ASPNETCore.Infrastructure;

namespace WWC._240711.ASPNETCore.Ocelot
{
    public static class CXLOcelotExtensions
    {
        public static IServiceCollection AddCXLOcelot(this IServiceCollection services)
        {
            var addOcelot = services.AddOcelot();

            if (Appsettings.app<bool>("UseConsul"))
            {
                addOcelot.AddConsul();
            }
            return services;
        }

        public static IConfigurationBuilder AddCXLOcelotConfigure(this ConfigurationManager configuration)
        {
            var con = configuration.AddJsonFile("ocelot.global.json", false, true);
            if (Appsettings.app<bool>("UseConsul"))
            {
                con.AddJsonFile("ocelot.consul.json");
            }
            con.AddJsonFile("ocelot.routes.json");
            con.AddJsonFile("ocelot.auth.json");
            con.AddJsonFile("ocelot.swagger.json");

            return con;
        }

        public async static Task<IApplicationBuilder> UseCXLOcelot(this WebApplication app)
        {
            return await app.UseOcelot();
        }

    }
}
