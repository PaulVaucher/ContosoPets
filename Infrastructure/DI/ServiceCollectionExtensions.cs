using ContosoPets.Application.Ports;
using ContosoPets.Application.Services;
using ContosoPets.Application.SharedKernel;
using ContosoPets.Domain.Constants;
using ContosoPets.Domain.Services;
using ContosoPets.Infrastructure.Configuration;
using ContosoPets.Infrastructure.Database;
using ContosoPets.Infrastructure.Output;
using ContosoPets.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHibernate;
using System.Reflection;

namespace ContosoPets.Infrastructure.DI
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configure all infrastructure services
        /// </summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAnimalDomainService, AnimalDomainService>();
            services.AddScoped<IAnimalApplicationService, AnimalApplicationService>();

            services.AddScoped<IAnimalRepository, AnimalRepository>();
            services.AddSingleton<ILinePrinter, ConsoleLinePrinter>();

            services.AddLogging(ConfigureLogging);

            services.AddSingleton<ISessionFactory>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<ISessionFactory>>();
                logger.LogInformation(LoggingConstants.ConfigurationLoaded, "NHibernate SessionFactory");

                var nhConfig = new NHibernateConfiguration(configuration);
                return nhConfig.CreateSessionFactory();
            });

            services.AddSingleton<DatabaseInitializer>();

            services.AddInjectablesFromAssembly(typeof(IAssemblyReference).Assembly);

            return services;
        }

        private static void ConfigureLogging(ILoggingBuilder builder)
        {
            builder.ClearProviders();

            // Debug output for Visual Studio - no console conflicts
            builder.AddDebug();

            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddFilter("Microsoft", LogLevel.Warning);
            builder.AddFilter("System", LogLevel.Warning);
            builder.AddFilter("NHibernate", LogLevel.Warning);
            builder.AddFilter("ContosoPets", LogLevel.Debug);
        }

        /// <summary>
        /// Scan the given assembly and automatically register all services
        /// </summary>
        public static IServiceCollection AddInjectablesFromAssembly(this IServiceCollection services, Assembly assembly)
        {
            services.Scan(scan => scan
                .FromAssemblies(assembly)
                .AddClasses(classes => classes.AssignableTo<IAssemblyReference>()
                    .Where(type => !type.IsInterface && !type.IsAbstract))
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

            return services;
        }
    }
}