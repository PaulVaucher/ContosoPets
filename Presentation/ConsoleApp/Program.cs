using Contosopets.Bootstrap;
using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Domain.Constants;
using ContosoPets.Infrastructure.AssemblyReferences;
using ContosoPets.Presentation.ConsoleApp.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace ContosoPets.Presentation.ConsoleApp
{
    static class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();                // 1. Créer la collection
            services.AddInjectablesFromAssembly(typeof(IAssemblyReference).Assembly); // 2. Enregistrer

            var serviceProvider = services.BuildServiceProvider(); // 3. Construire le conteneur

            var service = serviceProvider.GetRequiredService<IAnimalService>(); // 4. Résoudre une instance
            RunApplication(service);
        }

        private static void RunApplication(IAnimalService service)
        {
            
            Console.WriteLine(AppConstants.WelcomeMessage);

            bool exit = false;
            void ExitApp() => exit = true;

            var (orderedMenu, commandMap) = CommandRegistry.BuildCommandRegistry(service, ExitApp);

            while (!exit)
            {
                foreach (var entry in orderedMenu)
                {
                    Console.WriteLine(entry.Option.ToLabel());
                }

                if (int.TryParse(Console.ReadLine(), out int input) && Enum.IsDefined(typeof(MenuOptionEnum), input))
                {
                    var selected = (MenuOptionEnum)input;
                    Console.WriteLine();

                    if (commandMap.TryGetValue(selected, out var command))
                    {
                        command.Execute();
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
        }
    }
}