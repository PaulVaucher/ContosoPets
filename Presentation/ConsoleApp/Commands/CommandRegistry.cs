using ContosoPets.Application.Ports;
using ContosoPets.Application.Services;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public record MenuCommandEntry(MenuOptionEnum Option, IMenuCommand Command);

    public static class CommandRegistry
    {
        public static (List<MenuCommandEntry> OrderedList, Dictionary<MenuOptionEnum, IMenuCommand> Lookup)
        BuildCommandRegistry(IAnimalApplicationService service, ILinePrinter output, Action exitCallback)
        {
            var list = new List<MenuCommandEntry>
            {
                new(MenuOptionEnum.MenuListAllAnimals, new ListAllAnimalsCommand(service, output)),
                new(MenuOptionEnum.MenuAddNewAnimal, new AddNewAnimalCommand(service, output)),
                new(MenuOptionEnum.MenuEnsureAgesAndDescriptionsComplete, new EnsureAgesDescriptionsCommand(service, output)),
                new(MenuOptionEnum.MenuEnsureNicknamesAndPersonalityComplete, new EnsureNicknamesPersonalityCommand(service, output)),
                new(MenuOptionEnum.MenuEditAnimalAge, new EditAnimalAgeCommand(service, output)),
                new(MenuOptionEnum.MenuEditAnimalPersonality, new EditAnimalPersonalityCommand(service, output)),
                new(MenuOptionEnum.MenuDisplayCatsWithCharacteristic, new DisplayCatsWithCharacteristicCommand(service, output)),
                new(MenuOptionEnum.MenuDisplayDogsWithCharacteristic, new DisplayDogsWithCharacteristicCommand(service, output)),
                new(MenuOptionEnum.MenuExit, new ExitCommand(output, exitCallback))
            };

            var dict = list.ToDictionary(x => x.Option, x => x.Command);

            return (list, dict);
        }
    }
}