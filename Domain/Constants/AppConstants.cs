namespace ContosoPets.Domain.Constants
{
    public abstract class AppConstants
    {
        // General messages
        public const string WelcomeMessage = "Welcome to the Contoso PetFriends app. Your main menu options are:";
        public const string InvalidOptionMessage = "Invalid option. Please try again.";
        public const string GoodbyeMessage = "Goodbye!";
        public const string AnimalNotFoundMessage = "Animal not found.";
        public const string NoAnimalsFoundMessage = "No animals found.";

        // Input prompts
        public const string EnterSpeciesPrompt = "Enter 'dog' or 'cat' to begin a new entry\n";
        public const string InvalidSpeciesMessage = "Invalid input. Please enter 'dog' or 'cat'.";
        public const string AgePromptFormat = "Enter an age for the animal {0} or '?' if unknown";
        public const string AgePromptComplete = "Please enter an age for {0} ({1})";
        public const string PhysicalDescriptionPromptFormat = "Enter a physical description for the animal {0} (size, color, breed, gender, weight, housebroken)";
        public const string PhysicalDescriptionPromptComplete = "Please enter a physical description for {0} ({1})";
        public const string PersonalityDescriptionPromptFormat = "Enter a personality description for the animal {0} (likes or dislikes, tricks, energy level)";
        public const string NicknamePromptFormat = "Enter a nickname for the animal {0} or tbd if unknown";
        public const string EnterAnimalIdPrompt = "Enter the ID of the animal whose {0} you want to edit:";
        public const string CharacteristicSearchPromptFormat = "Enter the characteristic to search for in {0}s:";

        // Status messages
        public const string PetLimitReachedMessage = "We have reached our limit on the number of pets that we can manage.";
        public const string CurrentPetsStatusFormat = "We currently have {0} pets that need homes. We can manage {1} more.";
        public const string AddAnotherPetPrompt = "Do you want to enter info for another pet (y/n)";
        public const string InvalidYesNoMessage = "Invalid input. Please enter 'y' or 'n'.";
        public const string AgeAndDescriptionCompleteMessage = "Age and physical description information is complete for all animals.";
        public const string NicknamePersonalityCompleteMessage = "Nickname and personality information is complete for all animals.";

        // Update messages
        public const string CurrentAgeFormat = "Current age for {0} is {1}.\nEnter new age or '?' if unknown:";
        public const string UpdatedAgeFormat = "Updated age for {0} to {1}.\nEnter new personality description:";
        public const string CurrentPersonalityFormat = "Current personality for {0}: {1}";
        public const string UpdatedPersonalityMessage = "Personality updated successfully.";

        // Search results
        public const string CharacteristicResultsFormat = "{0}s with characteristic '{1}':";
        public const string NoCharacteristicMatchFormat = "No {0}s found with characteristic '{1}'.";    
    }
}
