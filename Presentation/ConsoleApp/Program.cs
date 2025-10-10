using ContosoPets.Domain.Constants;
using ContosoPets.Infrastructure.StartUp;
using ContosoPets.Presentation.ConsoleApp.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ContosoPets.Presentation.ConsoleApp
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var serviceProvider = ServiceContainer.ConfigureServices();
                using (serviceProvider)
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<object>>();
                    logger.LogInformation(LoggingConstants.ApplicationStarting);

                    await DatabaseStartup.InitializeDatabaseAsync(serviceProvider);
                    ApplicationRunner.RunApplication(serviceProvider, logger);

                    logger.LogInformation(LoggingConstants.ApplicationShuttingDown);
                }
            }
            catch (Exception ex)
            {                
                Console.WriteLine(string.Format(AppConstants.ApplicationStartupErrorFormat, ex.Message));
                Console.WriteLine(AppConstants.ApplicationExitingMessage);
                Environment.ExitCode = 1;
            }
        }               
    }
}