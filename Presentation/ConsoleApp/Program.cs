using ContosoPets.Application.UseCases.Animals;
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
                var settingsFile = Path.Combine(path!, "appsettings.json");
                if (File.Exists(settingsFile))
                {
                    configPath = path;
                    break;
                }
            }

            if (configPath != null)
            {
                builder.SetBasePath(configPath)
                       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                       .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development"}.json", optional: true, reloadOnChange: true);
            }
            else
            {                
                Console.WriteLine("Fichiers de configuration non trouvés, utilisation de la configuration par défaut.");
                builder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Port=5432;Database=contoso_pets;Username=postgres;Password=postgres",
                    ["NHibernate:ShowSql"] = "true",
                    ["NHibernate:FormatSql"] = "true",
                    ["NHibernate:SchemaAction"] = "create-drop"
                });
            }

            return builder.Build();
        }

        private static void RunApplication(ServiceProvider serviceProvider)
        {
            try
            {
                var animalService = serviceProvider.GetRequiredService<IAnimalService>();
                RunInteractiveMenu(animalService);
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

        private static void RunInteractiveMenu(IAnimalService service)
        {
            Console.WriteLine(AppConstants.WelcomeMessage);

            bool exit = false;
            void ExitApp() => exit = true;

            var (orderedMenu, commandMap) = CommandRegistry.BuildCommandRegistry(service, ExitApp);

            while (!exit)
            {
                try
                {
                    DisplayMenu(orderedMenu);
                    ProcessUserInput(commandMap);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format(AppConstants.MenuExecutionErrorFormat, ex.Message));
                    Console.WriteLine(AppConstants.ContinuePrompt);
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        private static void DisplayMenu(List<MenuCommandEntry> orderedMenu)
        {
            Console.WriteLine();
            foreach (var entry in orderedMenu)
            {
                Console.WriteLine(entry.Option.ToLabel());
            }
            Console.Write(AppConstants.MenuPrompt);
        }

        private static void ProcessUserInput(Dictionary<MenuOptionEnum, IMenuCommand> commandMap)
        {
            var input = Console.ReadLine();

            if (int.TryParse(input, out int menuChoice) &&
                Enum.IsDefined(typeof(MenuOptionEnum), menuChoice))
            {
                var selected = (MenuOptionEnum)menuChoice;
                Console.WriteLine();

                if (commandMap.TryGetValue(selected, out var command))
                {
                    ExecuteCommand(command);
                }
                else
                {
                    Console.WriteLine(AppConstants.InvalidOptionMessage);
                }
            }
            else
            {
                Console.WriteLine(AppConstants.InvalidOptionMessage);
            }
        }

        private static void ExecuteCommand(IMenuCommand command)
        {
            try
            {
                command.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(AppConstants.MenuExecutionErrorFormat, ex.Message));
                Console.WriteLine(AppConstants.ContinuePrompt);
                Console.ReadKey();
            }
        }
    }
}