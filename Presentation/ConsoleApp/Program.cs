using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Infrastructure.Repositories;
using ContosoPets.Infrastructure.Services;
using ContosoPets.Domain.Constants;

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
            while (!exit)
            {
                Console.WriteLine(MenuOptionEnum.MenuListAllAnimals.ToLabel());
                Console.WriteLine(MenuOptionEnum.MenuAddNewAnimal.ToLabel());
                Console.WriteLine(MenuOptionEnum.MenuEnsureAgesAndDescriptionsComplete.ToLabel());
                Console.WriteLine(MenuOptionEnum.MenuEnsureNicknamesAndPersonalityComplete.ToLabel());
                Console.WriteLine(MenuOptionEnum.MenuEditAnimalAge.ToLabel());
                Console.WriteLine(MenuOptionEnum.MenuEditAnimalPersonality.ToLabel());
                Console.WriteLine(MenuOptionEnum.MenuDisplayCatsWithCharacteristic.ToLabel());
                Console.WriteLine(MenuOptionEnum.MenuDisplayDogsWithCharacteristic.ToLabel());
                Console.WriteLine(MenuOptionEnum.MenuExit.ToLabel());

                if (int.TryParse(Console.ReadLine(), out int input) && Enum.IsDefined(typeof(MenuOptionEnum), input))
                {
                    var menuOption = (MenuOptionEnum)input;
                    Console.WriteLine();

                    switch (menuOption)
                    {
                        case MenuOptionEnum.MenuListAllAnimals:
                            var animals = service.ListAll();
                            if (animals.Count == 0)
                            {
                                Console.WriteLine(AppConstants.NoAnimalsFoundMessage);
                            }
                            else
                            {
                                foreach (var animal in animals)
                                {
                                    animal.DisplayInfo();
                                    Console.WriteLine();
                                }
                            }
                            break;
                        case MenuOptionEnum.MenuAddNewAnimal:
                            service.AddNewAnimal();
                            break;
                        case MenuOptionEnum.MenuEnsureAgesAndDescriptionsComplete:
                            service.EnsureAgesAndDescriptionsComplete();
                            break;
                        case MenuOptionEnum.MenuEnsureNicknamesAndPersonalityComplete:
                            service.EnsureNicknamesAndPersonalityComplete();
                            break;
                        case MenuOptionEnum.MenuEditAnimalAge:
                            service.EditAnimalAge();
                            break;
                        case MenuOptionEnum.MenuEditAnimalPersonality:
                            service.EditAnimalPersonality();
                            break;
                        case MenuOptionEnum.MenuDisplayCatsWithCharacteristic:
                            service.DisplayCatsWithCharacteristic();
                            break;
                        case MenuOptionEnum.MenuDisplayDogsWithCharacteristic:
                            service.DisplayDogsWithCharacteristic();
                            break;
                        case MenuOptionEnum.MenuExit:
                            exit = true;
                            Console.WriteLine(AppConstants.GoodbyeMessage);
                            break;
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