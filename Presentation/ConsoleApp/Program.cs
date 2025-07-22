using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Domain.Constants;
using ContosoPets.Infrastructure.Repositories;
using ContosoPets.Infrastructure.Services;
using ContosoPets.Presentation.ConsoleApp.Commands;

namespace ContosoPets.Presentation.ConsoleApp
{
    static class Program
    {
        static void Main(string[] args)
        {
            RunApplication();
        }

        private static void RunApplication()
        {
            var repository = new AnimalRepository();
            IAnimalService service = new AnimalService(repository);

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