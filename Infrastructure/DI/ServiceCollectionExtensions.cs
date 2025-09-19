using ContosoPets.Application.Ports;
using ContosoPets.Application.Services;
using ContosoPets.Application.SharedKernel;
using ContosoPets.Domain.Services;
using ContosoPets.Infrastructure.Output;
using ContosoPets.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ContosoPets.Infrastructure.DI
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures all infrastructure services and dependencies
        /// </summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IAnimalDomainService, AnimalDomainService>();
            services.AddScoped<IAnimalApplicationService, AnimalApplicationService>();
            services.AddScoped<IAnimalRepository, AnimalRepository>();
            services.AddScoped<ILinePrinter, ConsoleLinePrinter>();

            services.AddInjectablesFromAssembly(typeof(IAssemblyReference).Assembly);

            return services;
        }

        /// <summary>
        /// Scans the given assembly and automatically registers infrastructure services
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