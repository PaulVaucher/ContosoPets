using ContosoPets.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ContosoPets.Domain.DI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IAnimalDomainService, AnimalDomainService>();
            
            return services;
        }
    }
}
