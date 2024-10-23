using Microsoft.AspNetCore.Builder;
using Ocelot.Authentication.Middleware;
using Ocelot.Cache.Middleware;
using Ocelot.Claims.Middleware;
using Ocelot.DependencyInjection;
using Ocelot.DownstreamRouteFinder.Middleware;
using Ocelot.DownstreamUrlCreator.Middleware;
using Ocelot.Errors.Middleware;
using Ocelot.Headers.Middleware;
using Ocelot.LoadBalancer.Middleware;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.QueryStrings.Middleware;
using Ocelot.RateLimiting.Middleware;
using Ocelot.Request.Middleware;
using Ocelot.Requester.Middleware;
using Ocelot.RequestId.Middleware;
using Ocelot.Responder.Middleware;
using Ocelot.Security.Middleware;
using Ocelot.WebSockets;
using WWC._240711.ASPNETCore.Infrastructure;

namespace WWC._240711.ASPNETCore.Ocelot
{
    public static class CXLOcelotExtensions
    {
        public static IServiceCollection AddCXLOcelot(this IServiceCollection services)
        {
            var addOcelot = services.AddOcelot();

            if (Appsettings.app<bool>("UseOcelotConsul"))
            {
                addOcelot.AddConsul();
            }
            return services;
        }

        public static IConfigurationBuilder AddCXLOcelotConfigure(this ConfigurationManager configuration)
        {
            var con = configuration
                .AddJsonFile("ocelot.global.json", false, true)
                .AddJsonFile("ocelot.json", false, true);
            if (Appsettings.app<bool>("UseOcelotConsul"))
            {
                con.AddJsonFile("ocelot.consul.json");
            }
            //con.AddJsonFile("ocelot.routes.json", false, true);
            //con.AddJsonFile("ocelot.auth.json", false, true);
            //con.AddJsonFile("ocelot.swagger.json", false, true);

            return con;
        }

        public async static Task<IApplicationBuilder> UseCXLOcelot(this WebApplication app)
        {
            return await app.UseOcelot(config =>
            {

            });
            //return await app.UseOcelot((builder, config) =>
            //{
            //    builder.UseResponderMiddleware();
            //});
        }

    //    /// <summary>
    //    /// 使用 Ocelot 中间件配置
    //    /// </summary>
    //    /// <param name="builder"></param>
    //    /// <param name="pipelineConfiguration"></param>
    //    /// <returns></returns>
    //    public static IApplicationBuilder BuildCustomOcelotPipeline(this IApplicationBuilder builder,
    //OcelotPipelineConfiguration pipelineConfiguration)
    //    {
    //        builder.UseExceptionHandlerMiddleware();
    //        builder.MapWhen(context => context.WebSockets.IsWebSocketRequest,
    //            app =>
    //            {
    //                app.UseDownstreamRouteFinderMiddleware();
    //                app.UseDownstreamRequestInitialiser();
    //                app.UseLoadBalancingMiddleware();
    //                app.UseDownstreamUrlCreatorMiddleware();
    //                app.UseWebSocketsProxyMiddleware();
    //            });
    //        //builder.UseIfNotNull(pipelineConfiguration.PreErrorResponderMiddleware);
    //        builder.UseResponderMiddleware();
    //        builder.UseDownstreamRouteFinderMiddleware();
    //        builder.UseSecurityMiddleware();
    //        //if (pipelineConfiguration.MapWhenOcelotPipeline != null)
    //        //{
    //        //    foreach (var pipeline in pipelineConfiguration.MapWhenOcelotPipeline)
    //        //    {
    //        //        builder.MapWhen(pipeline);
    //        //    }
    //        //}
    //        builder.UseHttpHeadersTransformationMiddleware();
    //        builder.UseDownstreamRequestInitialiser();
    //        builder.UseRateLimiting();

    //        builder.UseRequestIdMiddleware();
    //        //builder.UseIfNotNull(pipelineConfiguration.PreAuthenticationMiddleware);
    //        if (pipelineConfiguration.AuthenticationMiddleware == null)
    //        {
    //            builder.UseAuthenticationMiddleware();
    //        }
    //        else
    //        {
    //            builder.Use(pipelineConfiguration.AuthenticationMiddleware);
    //        }
    //        builder.UseClaimsToClaimsMiddleware();
    //        //builder.UseIfNotNull(pipelineConfiguration.PreAuthorisationMiddleware);
    //        //if (pipelineConfiguration.AuthorisationMiddleware == null)
    //        //{
    //        //    builder.UseAuthorisationMiddleware();
    //        //}
    //        //else
    //        //{
    //        //    builder.Use(pipelineConfiguration.AuthorisationMiddleware);
    //        //}
    //        builder.UseClaimsToHeadersMiddleware();
    //        //builder.UseIfNotNull(pipelineConfiguration.PreQueryStringBuilderMiddleware);
    //        builder.UseClaimsToQueryStringMiddleware();
    //        builder.UseLoadBalancingMiddleware();
    //        builder.UseDownstreamUrlCreatorMiddleware();
    //        builder.UseOutputCacheMiddleware();
    //        builder.UseHttpRequesterMiddleware();

            
    //        builder.UseAuthentication();
    //        builder.UseAuthorization();
    //        return builder;
    //    }


    }
}
