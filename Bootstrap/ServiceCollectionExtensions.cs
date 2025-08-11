using ContosoPets.Infrastructure.AssemblyReferences;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Contosopets.Bootstrap
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Scan l'assembly donné et enregistre automatiquement tous les services et repositories
        /// qui implémentent les interfaces marqueurs IInjectableService et IInjectableRepository.
        /// </summary>
        public static IServiceCollection AddInjectablesFromAssembly(this IServiceCollection services, Assembly assembly)
        {
            services.Scan(scan => scan
               .FromAssemblies(assembly)
               .AddClasses(classes => classes.AssignableTo<IAssemblyReference>())
               .AsImplementedInterfaces()
               .WithSingletonLifetime());

            return services;
        }
    }
}
