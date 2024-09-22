using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions
{
    public static class CXLServiceExtensions
    {

        public static WebApplicationBuilder AddCXLServiceContainer(this WebApplicationBuilder builder)
        {
            builder.Host.UseServiceProviderFactory<ICXLServiceContainer>(new CXLServiceProviderFactory());
            builder.Host.ConfigureContainer<ICXLServiceContainer>((context, container) =>
            {

            });

            builder.Services.AddSingleton<IDeveloperPageExceptionFilter, CXLDeveloperPageExceptionFilter>();

            return builder;
        }

    }
}
