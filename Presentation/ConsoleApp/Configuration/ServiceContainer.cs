using ContosoPets.Domain.Constants;
using ContosoPets.Infrastructure.DI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContosoPets.Presentation.ConsoleApp.Configuration
{
    public static class ServiceContainer
    {
        public static ServiceProvider ConfigureServices()
        {
            try
            {
                var services = new ServiceCollection();

                var configuration = Configuration.AppConfigurationBuilder.BuildConfiguration();

                services.AddSingleton<IConfiguration>(configuration);

                services.AddInfrastructure(configuration);
                return services.BuildServiceProvider();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    string.Format(AppConstants.ServiceConfigurationErrorFormat, ex.Message), ex);
            }
        }        
    }
}
