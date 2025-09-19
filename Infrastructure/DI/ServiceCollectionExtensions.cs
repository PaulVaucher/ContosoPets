using ContosoPets.Application.DI;
using ContosoPets.Application.Ports;
using ContosoPets.Application.SharedKernel;
using ContosoPets.Domain.DI;
using ContosoPets.Infrastructure.Configuration;
using ContosoPets.Infrastructure.Database;
using ContosoPets.Infrastructure.Output;
using ContosoPets.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddDomainServices();
            services.AddApplicationServices();

            services.AddLogging();

            services.AddSingleton<ISessionFactory>(provider =>
            {
                var nhConfig = new NHibernateConfiguration(configuration);
                return nhConfig.CreateSessionFactory();
            });

            services.AddScoped<IAnimalRepository, AnimalRepository>();
            services.AddSingleton<ILinePrinter, ConsoleLinePrinter>();
            services.AddSingleton<DatabaseInitializer>();

            services.AddInjectablesFromAssembly(typeof(IAssemblyReference).Assembly);

            return services;
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