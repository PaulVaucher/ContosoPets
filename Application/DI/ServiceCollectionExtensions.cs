using ContosoPets.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ContosoPets.Application.DI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAnimalApplicationService, AnimalApplicationService>();
            
            return services;
        }
    }
}
