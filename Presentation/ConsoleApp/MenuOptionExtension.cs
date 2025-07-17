namespace ContosoPets.Presentation.ConsoleApp
{
    public static class MenuOptionExtension
    {
        public static string ToLabel(this MenuOptionEnum type)
        {
            return type switch
            {
                MenuOptionEnum.MenuListAllAnimals => "1. List all of our current pet information",
                MenuOptionEnum.MenuAddNewAnimal => "2. Add a new animal friend to the application",
                MenuOptionEnum.MenuEnsureAgesAndDescriptionsComplete => "3. Ensure animal ages and physical descriptions are complete",
                MenuOptionEnum.MenuEnsureNicknamesAndPersonalityComplete => "4. Ensure animal nicknames and personality descriptions are complete",
                MenuOptionEnum.MenuEditAnimalAge => "5. Edit an animal's age",
                MenuOptionEnum.MenuEditAnimalPersonality => "6. Edit an animal's personality description",
                MenuOptionEnum.MenuDisplayCatsWithCharacteristic => "7. Display all cats with a specified characteristic",
                MenuOptionEnum.MenuDisplayDogsWithCharacteristic => "8. Display all dogs with a specified characteristic",
                MenuOptionEnum.MenuExit => "0. Exit the application",
                _ => type.ToString()
            };
        }
    }
}
