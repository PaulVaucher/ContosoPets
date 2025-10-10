using ContosoPets.Domain.Constants;
using ContosoPets.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ContosoPets.Infrastructure.StartUp
{
    public static class DatabaseStartup
    {
        public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<object>>();
            logger.LogDebug(LoggingConstants.DatabaseInitializationStarted);

            var initializer = serviceProvider.GetRequiredService<DatabaseInitializer>();
            await initializer.InitializeDatabaseAsync();

            logger.LogInformation(LoggingConstants.DatabaseInitializationCompleted);
        }
    }
}
