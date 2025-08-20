using ContosoPets.Infrastructure.AssemblyReferences;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ContosoPets.Infrastructure.DI
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configure tous les services de l'infrastructure
        /// </summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddInjectablesFromAssembly(typeof(IAssemblyReference).Assembly);
            return services;
        }

        /// <summary>
        /// Scan l'assembly donné et enregistre automatiquement tous les services
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