using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.WebHost.Custom.Services
{
    public static class CustomServiceExtensions
    {
        public static IServiceCollection CustomServicesSetup(this IServiceCollection services)
        {


            return services;
        }
    }
}
