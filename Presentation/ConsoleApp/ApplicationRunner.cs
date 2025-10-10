using ContosoPets.Application.Ports;
using ContosoPets.Application.Services;
using ContosoPets.Domain.Constants;
using ContosoPets.Presentation.ConsoleApp.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ContosoPets.Presentation.ConsoleApp
{
    public static class ApplicationRunner
    {
        public static void RunApplication(ServiceProvider serviceProvider, ILogger<object> logger)
        {
            var animalService = serviceProvider.GetRequiredService<IAnimalApplicationService>();
            var output = serviceProvider.GetRequiredService<ILinePrinter>();
            var menuLogger = serviceProvider.GetRequiredService<ILogger<MenuHandler>>();

            logger.LogInformation(LoggingConstants.ApplicationStarted);
            
            var menuHandler = new MenuHandler(menuLogger, animalService, output);
            menuHandler.RunInteractiveMenu();
        }
    }
}
