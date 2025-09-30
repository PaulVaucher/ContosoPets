using ContosoPets.Application.Ports;
using ContosoPets.Application.Services;
using ContosoPets.Domain.Constants;
using ContosoPets.Infrastructure.Database;
using ContosoPets.Infrastructure.DI;
using ContosoPets.Presentation.ConsoleApp.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ContosoPets.Presentation.ConsoleApp
{
    static class Program
    {
        private static ILogger<object>? _logger;
        private static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<object>>();
            logger.LogDebug(LoggingConstants.DatabaseInitializationStarted);

            var initializer = serviceProvider.GetRequiredService<DatabaseInitializer>();
            await initializer.InitializeDatabaseAsync();

            logger.LogInformation(LoggingConstants.DatabaseInitializationCompleted);
        }

        static async Task Main(string[] args)
        {
            try
            {
                var serviceProvider = ConfigureServices();
                using (serviceProvider)
                {
                    _logger = serviceProvider.GetRequiredService<ILogger<object>>();
                    _logger.LogInformation(LoggingConstants.ApplicationStarting);

                    await InitializeDatabaseAsync(serviceProvider);
                    RunApplication(serviceProvider);

                    _logger.LogInformation(LoggingConstants.ApplicationShuttingDown);
                }
            }
            catch (Exception ex)
            {
                // Keep Console.WriteLine for critical startup errors since logs might not be available
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
                var animalService = serviceProvider.GetRequiredService<IAnimalApplicationService>();
                var output = serviceProvider.GetRequiredService<ILinePrinter>();

                _logger?.LogInformation(LoggingConstants.ApplicationStarted);
                RunInteractiveMenu(animalService, output);            
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
                    _logger?.LogError(ex, LoggingConstants.UnexpectedError, nameof(Program), nameof(RunInteractiveMenu));
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

            _logger?.LogDebug(LoggingConstants.UserInputReceived, LoggingConstants.InputTypeMenuSelection, input ?? LoggingConstants.InputValueNull);

            if (int.TryParse(input, out int menuChoice) &&
                Enum.IsDefined(typeof(MenuOptionEnum), menuChoice))
            {
                var selected = (MenuOptionEnum)menuChoice;
                output.PrintLine();

                _logger?.LogInformation(LoggingConstants.UserMenuSelection, selected);

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
                _logger?.LogWarning(LoggingConstants.InvalidUserInput, LoggingConstants.InputTypeMenuSelection, input ?? LoggingConstants.InputValueNull);
                output.PrintLine(AppConstants.InvalidOptionMessage);
            }
        }

        private static void ExecuteCommand(IMenuCommand command, ILinePrinter output)
        {
            try
            {
                using var scope = _logger?.BeginScope("Command={CommandType}", command.GetType().Name);
                _logger?.LogDebug(LoggingConstants.ExecutingCommand, command.GetType().Name);

                command.Execute();

                _logger?.LogDebug(LoggingConstants.CommandExecutedSuccessfully, command.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, LoggingConstants.CommandExecutionFailed, command.GetType().Name);
                output.PrintLine(string.Format(AppConstants.MenuExecutionErrorFormat, ex.Message));
                output.PrintLine(AppConstants.ContinuePrompt);
                output.ReadKey();
            }
        }
    }
}