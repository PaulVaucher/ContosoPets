using ContosoPets.Application.Ports;
using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Domain.Constants;
using ContosoPets.Infrastructure.DI;
using ContosoPets.Presentation.ConsoleApp.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace ContosoPets.Presentation.ConsoleApp
{
    static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var serviceProvider = ConfigureServices();
                using (serviceProvider)
                {
                    var animalService = serviceProvider.GetRequiredService<IAnimalService>();
                    var output = serviceProvider.GetRequiredService<ILinePrinter>();

                    RunApplication(animalService, output);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(AppConstants.ApplicationStartupErrorFormat, ex.Message));
                Console.WriteLine(AppConstants.ApplicationExitingMessage);
                Environment.ExitCode = 1;
            }
        }

        private static ServiceProvider ConfigureServices()
        {
            try
            {
                var services = new ServiceCollection();
                services.AddInfrastructure();
                return services.BuildServiceProvider();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    string.Format(AppConstants.ServiceConfigurationErrorFormat, ex.Message), ex);
            }
        }

        private static void RunApplication(IAnimalService animalService, ILinePrinter output)
        {
            try
            {                
                RunInteractiveMenu(animalService, output);
            }
            catch (InvalidOperationException ex)
            {
                output.PrintLine(string.Format(AppConstants.ServiceConfigurationErrorFormat, ex.Message));
                throw;
            }
            catch (Exception ex)
            {
                output.PrintLine(string.Format(AppConstants.UnexpectedErrorFormat, ex.Message));
                throw;
            }
        }

        private static void RunInteractiveMenu(IAnimalService service, ILinePrinter output)
        {
            output.PrintLine(AppConstants.WelcomeMessage);

            bool exit = false;
            void ExitApp() => exit = true;

            var (orderedMenu, commandMap) = CommandRegistry.BuildCommandRegistry(service, output, ExitApp);

            while (!exit)
            {
                try
                {
                    DisplayMenu(orderedMenu, output);
                    ProcessUserInput(commandMap, output);
                }
                catch (Exception ex)
                {
                    output.PrintLine(string.Format(AppConstants.MenuExecutionErrorFormat, ex.Message));
                    output.PrintLine(AppConstants.ContinuePrompt);
                    output.ReadKey();
                    output.Clear();
                }
            }
        }

        private static void DisplayMenu(List<MenuCommandEntry> orderedMenu, ILinePrinter output)
        {
            output.PrintLine();
            foreach (var entry in orderedMenu)
            {
                output.PrintLine(entry.Option.ToLabel());
            }
            output.Write(AppConstants.MenuPrompt);
        }

        private static void ProcessUserInput(Dictionary<MenuOptionEnum, IMenuCommand> commandMap, ILinePrinter output)
        {
            var input = output.ReadLine();

            if (int.TryParse(input, out int menuChoice) &&
                Enum.IsDefined(typeof(MenuOptionEnum), menuChoice))
            {
                var selected = (MenuOptionEnum)menuChoice;
                output.PrintLine();

                if (commandMap.TryGetValue(selected, out var command))
                {
                    ExecuteCommand(command, output);
                }
                else
                {
                    output.PrintLine(AppConstants.InvalidOptionMessage);
                }
            }
            else
            {
                output.PrintLine(AppConstants.InvalidOptionMessage);
            }
        }

        private static void ExecuteCommand(IMenuCommand command, ILinePrinter output)
        {
            try
            {
                command.Execute();
            }
            catch (Exception ex)
            {
                output.PrintLine(string.Format(AppConstants.MenuExecutionErrorFormat, ex.Message));
                output.PrintLine(AppConstants.ContinuePrompt);
                output.ReadKey();
            }
        }
    }
}