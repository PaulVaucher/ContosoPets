using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Domain.Constants;
using ContosoPets.Domain.Entities;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class AddNewAnimalCommand(IAnimalService service) : IMenuCommand
    {
        private readonly IAnimalService _service = service;

        public void Execute()
        {
            var animals = _service.ListAll();
            int petCount = animals.Count(a => !string.IsNullOrEmpty(a.Id));
            const int MaxPets = 8;
            const string CanYes = "y";
            const string CanNo = "n";

            if (petCount >= MaxPets)
            {
                Console.WriteLine(AppConstants.PetLimitReachedMessage);
                return;
            }

            Console.WriteLine(string.Format(AppConstants.CurrentPetsStatusFormat, petCount, MaxPets - petCount));
            string anotherPet = CanYes;

            while (anotherPet == CanYes && petCount < MaxPets)
            {
                Console.WriteLine(AppConstants.EnterSpeciesPrompt);
                var species = Console.ReadLine() ?? string.Empty;

                var id = Animal.GenerateId(species, petCount + 1);

                Console.WriteLine(string.Format(AppConstants.AgePromptFormat, id));
                var age = Console.ReadLine();

                Console.WriteLine(string.Format(AppConstants.PhysicalDescriptionPromptFormat, id));
                var physical = Console.ReadLine();

                Console.WriteLine(string.Format(AppConstants.PersonalityDescriptionPromptFormat, id));
                var personality = Console.ReadLine();

                Console.WriteLine(string.Format(AppConstants.NicknamePromptFormat, id));
                var nickname = Console.ReadLine();

                var request = new AddAnimalRequest
                {
                    Species = species ?? string.Empty,
                    Age = age ?? string.Empty,
                    PhysicalDescription = physical ?? string.Empty,
                    PersonalityDescription = personality ?? string.Empty,
                    Nickname = nickname ?? string.Empty
                };

                var result = _service.AddNewAnimal(request);

                if (result.Success)
                {
                    Console.WriteLine($"Successfully added new {species} with ID: {result.Animal?.Id}");
                    petCount++;

                }
                else
                {
                    Console.WriteLine($"Failed to add new animal: {result.ErrorMessage}");
                }

                if (petCount < MaxPets)
                {
                    Console.WriteLine(AppConstants.AddAnotherPetPrompt);
                    anotherPet = Console.ReadLine()?.ToLower() == CanYes ? CanYes : CanNo;
                }
                else
                {
                    Console.WriteLine(AppConstants.PetLimitReachedMessage);
                }
            }
        }
    }
}