using ContosoPets.Application.Ports;
using ContosoPets.Application.Services;
using ContosoPets.Domain.Constants;
using ContosoPets.Infrastructure.Database;
using ContosoPets.Infrastructure.DI;
using ContosoPets.Presentation.ConsoleApp.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ContosoPets.Presentation.ConsoleApp
{
    static class Program
    {
        private static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
        {
            var initializer = serviceProvider.GetRequiredService<DatabaseInitializer>();
            await initializer.InitializeDatabaseAsync();
        }

        static async Task Main(string[] args)
        {
            try
            {
                var serviceProvider = ConfigureServices();
                using (serviceProvider)
                {
                    await InitializeDatabaseAsync(serviceProvider);
                    RunApplication(serviceProvider);
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

                var configuration = BuildConfiguration();

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

        private static IConfiguration BuildConfiguration()
        {
            var builder = new ConfigurationBuilder();

            var possiblePaths = new[]
            {
                AppContext.BaseDirectory,
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                Directory.GetCurrentDirectory(),
                Path.Combine(Directory.GetCurrentDirectory(), "Presentation", "ConsoleApp"),
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Presentation", "ConsoleApp")
            };

            string? configPath = null;
            foreach (var path in possiblePaths.Where(p => !string.IsNullOrEmpty(p)))
            {
                var settingsFile = Path.Combine(path!, ProgramConstants.AppSettingsFileName);
                if (File.Exists(settingsFile))
                {
                    configPath = path;
                    break;
                }
            }

            if (configPath != null)
            {
                builder.SetBasePath(configPath)
                       .AddJsonFile(ProgramConstants.AppSettingsFileName, optional: false, reloadOnChange: true)
                       .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable(ProgramConstants.EnvironmentVariable) ?? ProgramConstants.DevelopmentEnvironment}.json", optional: true, reloadOnChange: true);
            }
            else
            {                
                Console.WriteLine(ProgramConstants.ConfigurationFilesNotFoundMessage);
                builder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = ProgramConstants.DefaultConnectionString,
                    ["NHibernate:ShowSql"] = ProgramConstants.DefaultShowSql,
                    ["NHibernate:FormatSql"] = ProgramConstants.DefaultFormatSql,
                    ["NHibernate:SchemaAction"] = ProgramConstants.DefaultSchemaAction
                });
            }

            return builder.Build();
        }

        private static void RunApplication(ServiceProvider serviceProvider)
        {
            try
            {
                var animalService = serviceProvider.GetRequiredService<IAnimalApplicationService>();
                var output = serviceProvider.GetRequiredService<ILinePrinter>();

                RunInteractiveMenu(animalService, output);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(string.Format(AppConstants.ServiceConfigurationErrorFormat, ex.Message));
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(AppConstants.UnexpectedErrorFormat, ex.Message));
                throw;
            }
        }

        private static void RunInteractiveMenu(IAnimalApplicationService service, ILinePrinter output)
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