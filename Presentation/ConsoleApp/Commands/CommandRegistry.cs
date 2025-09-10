using ContosoPets.Application.UseCases.Animals;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public record MenuCommandEntry(MenuOptionEnum Option, IMenuCommand Command);

    public static class CommandRegistry
    {
        public static (List<MenuCommandEntry> OrderedList, Dictionary<MenuOptionEnum, IMenuCommand> Lookup)
        BuildCommandRegistry(IAnimalService service, Action exitCallback)
        {
            var list = new List<MenuCommandEntry>
            {
                new(MenuOptionEnum.MenuListAllAnimals, new ListAllAnimalsCommand(service)),
                new(MenuOptionEnum.MenuAddNewAnimal, new AddNewAnimalCommand(service)),
                new(MenuOptionEnum.MenuEnsureAgesAndDescriptionsComplete, new EnsureAgesDescriptionsCommand(service)),
                new(MenuOptionEnum.MenuEnsureNicknamesAndPersonalityComplete, new EnsureNicknamesPersonalityCommand(service)),
                new(MenuOptionEnum.MenuEditAnimalAge, new EditAnimalAgeCommand(service)),
                new(MenuOptionEnum.MenuEditAnimalPersonality, new EditAnimalPersonalityCommand(service)),
                new(MenuOptionEnum.MenuDisplayCatsWithCharacteristic, new DisplayCatsWithCharacteristicCommand(service)),
                new(MenuOptionEnum.MenuDisplayDogsWithCharacteristic, new DisplayDogsWithCharacteristicCommand(service)),
                new(MenuOptionEnum.MenuExit, new ExitCommand(exitCallback))
            };

            var dict = list.ToDictionary(x => x.Option, x => x.Command);

            return (list, dict);
        }
    }
}