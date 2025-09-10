using ContosoPets.Infrastructure.AssemblyReferences;
using ContosoPets.Infrastructure.Configuration;
using ContosoPets.Infrastructure.Database;
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
            services.AddLogging();

            services.AddSingleton<ISessionFactory>(provider =>
            {
                var nhConfig = new NHibernateConfiguration(configuration);
                return nhConfig.CreateSessionFactory();
            });

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