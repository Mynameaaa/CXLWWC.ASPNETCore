using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace WWC._240711.ASPNETCore.Extensions.Startup.使用
{
    public class CustomStartup
    {
        public IConfiguration Configuration { get; set; }

        public CustomStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine("Root ConfigureServices");
            //注册服务

            //瞬态
            services.AddTransient<IPersonService, PersonService>();
            services.AddTransient<PersonService>();
            services.AddTransient(typeof(IPersonService), typeof(PersonService));
            services.AddTransient(typeof(PersonService));
            services.AddTransient<IPersonService>(factory =>
            {
                return new PersonService();
            });
            services.Add(new ServiceDescriptor(typeof(IPersonService), typeof(PersonService), ServiceLifetime.Transient));
            //如果没有则注册
            services.TryAdd(new ServiceDescriptor(typeof(IPersonService), typeof(PersonService), ServiceLifetime.Transient));
            //如果实现没有被注册
            services.TryAddEnumerable(new ServiceDescriptor(typeof(IPersonService), typeof(PersonService), ServiceLifetime.Transient));

            //作用域
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<PersonService>();
            services.AddScoped(typeof(IPersonService), typeof(PersonService));
            services.AddScoped(typeof(PersonService));
            services.AddScoped<IPersonService>(factory =>
            {
                return new PersonService();
            });
            services.Add(new ServiceDescriptor(typeof(IPersonService), typeof(PersonService), ServiceLifetime.Scoped));
            //如果没有则注册
            services.TryAdd(new ServiceDescriptor(typeof(IPersonService), typeof(PersonService), ServiceLifetime.Scoped));
            //如果实现没有被注册
            services.TryAddEnumerable(new ServiceDescriptor(typeof(IPersonService), typeof(PersonService), ServiceLifetime.Scoped));

            //单例
            services.AddSingleton<IPersonService, PersonService>();
            services.AddSingleton<PersonService>();
            services.AddSingleton(typeof(IPersonService), typeof(PersonService));
            services.AddSingleton(typeof(PersonService));
            services.AddSingleton<IPersonService>(factory =>
            {
                return new PersonService();
            });
            services.Add(new ServiceDescriptor(typeof(IPersonService), typeof(PersonService), ServiceLifetime.Singleton));
            //如果没有则注册
            services.TryAdd(new ServiceDescriptor(typeof(IPersonService), typeof(PersonService), ServiceLifetime.Singleton));
            //如果实现没有被注册
            services.TryAddEnumerable(new ServiceDescriptor(typeof(IPersonService), typeof(PersonService), ServiceLifetime.Singleton));

            //先注册的先执行
            services.AddTransient<IStartupFilter, StartupFilterOne>(); //注入StartupFilterOne
            services.AddTransient<IStartupFilter, StartupFilterTwo>(); //注入StartupFilterTwo
            services.AddTransient<IStartupFilter, StartupFilterThree>(); //注入StartupFilterThree

            services.AddControllersWithViews();

            services.AddSwaggerGen();

            //var provider = new CustomDatabaseConfigurationProvider();
            //provider.Load();
            //provider.TryGet("1:2", out string? Values);
            ////provider.TryGet("4", out string? Values2);

            ////IConfiguration configuration = new Configuration();
            //IConfigurationRoot configurationroot = new ConfigurationRoot(null);
            //IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            //IConfigurationProvider configurationProvider = new JsonConfigurationProvider(null);
            //IConfigurationSource configurationSource = new JsonConfigurationSource();
            //IConfigurationSection configurationSection = new ConfigurationSection(configurationroot, "user");
            //IConfigurationManager configurationManager = new ConfigurationManager();

        }
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            var confiugration = app.ApplicationServices.GetService<IConfiguration>();
            var type = confiugration.GetType();

            Console.WriteLine("Root Configure");

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

        }
    }
    internal class PersonService : IPersonService
    {

    }
    internal interface IPersonService
    {

    }
}
