using ContosoPets.Application.Ports;
using ContosoPets.Application.Services;
using ContosoPets.Domain.Constants;
using ContosoPets.Presentation.ConsoleApp.UI;
using Microsoft.Extensions.DependencyInjection;

namespace ContosoPets.Presentation.ConsoleApp
{
    public static class ApplicationRunner
    {
        public static void RunApplication(ServiceProvider serviceProvider)
        {
            try
            {
                var animalService = serviceProvider.GetRequiredService<IAnimalApplicationService>();
                var output = serviceProvider.GetRequiredService<ILinePrinter>();

                var menuHandler = new MenuHandler(animalService, output);
                menuHandler.RunInteractiveMenu();
            }
            catch (InvalidOperationException ex)
            {
                var output = serviceProvider.GetRequiredService<ILinePrinter>();
                output.PrintLine(string.Format(AppConstants.ServiceConfigurationErrorFormat, ex.Message));
                throw;
            }
            catch (Exception ex)
            {
                var output = serviceProvider.GetRequiredService<ILinePrinter>();
                output.PrintLine(string.Format(AppConstants.UnexpectedErrorFormat, ex.Message));
                throw;
            }
        }
    }
}
