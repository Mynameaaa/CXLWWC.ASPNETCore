using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Infrastructure
{
    public class Appsettings
    {
        private static IConfiguration configuration;

        public Appsettings(IConfiguration _c1onfiguration)
        {
            configuration = _c1onfiguration;
        }

        public static string app(string sectionKey)
        {
            return configuration[sectionKey] ?? string.Empty;
        }

        public static T? app<T>(string sectionKey)
        {
            return configuration.GetSection(sectionKey).Get<T>() ?? default;
        }

        public static IConfigurationSection appSection(string sectionKey)
        {
            return configuration.GetSection(sectionKey);
        }

    }
}
