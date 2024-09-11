using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Extensions.Logging.Custom;
using WWC._240711.ASPNETCore.Extensions.Logging.Custom.Db;

namespace WWC._240711.ASPNETCore.Extensions.Logging.Custom
{
    public static class ServiceExtensions
    {
        public static ILoggingBuilder AddCustomDatabaseProvider<T>(this WebApplicationBuilder builder, Action<DbLoggeroOptions> action = null) where T : LoggerDbContext, new()
        {
            var options = new DbLoggeroOptions();
            action?.Invoke(options);

            builder.Logging.AddProvider(new CustomDatabaseLoggerProvider<T>())
                .AddFilter<CustomDatabaseLoggerProvider<T>>((categoryName, level) =>
                {
                    if (options.UseDefaultFilter)
                    {
                        if (level >= LogLevel.Warning)
                            return true;
                        else
                            return false;
                    }

                    if (level >= options.MinLogLevel && (options.CategoryNames?.Contains(categoryName) ?? true))
                        return true;
                    else
                        return false;
                });
            return builder.Logging;
        }
    }
}
