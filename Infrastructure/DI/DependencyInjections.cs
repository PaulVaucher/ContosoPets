using Contosopets.Bootstrap;
using ContosoPets.Infrastructure.AssemblyReferences;
using Microsoft.Extensions.DependencyInjection;

namespace ContosoPets.Infrastructure.DI;

public static class DependencyInjections
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddInjectablesFromAssembly(typeof(IAssemblyReference).Assembly);
        return services;
    }
}