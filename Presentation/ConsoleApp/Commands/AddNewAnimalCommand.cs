using ContosoPets.Application.Services;
using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Application.Ports;
using ContosoPets.Domain.Constants;
using ContosoPets.Domain.Entities;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class AddNewAnimalCommand : IMenuCommand
    {
        private readonly IAnimalApplicationService _service;
        private readonly ILinePrinter _output;

        public AddNewAnimalCommand(IAnimalApplicationService service, ILinePrinter output)
        {
            _service = service;
            _output = output;
        }

        public void Execute()
        {
            var animals = _service.ListAll();
            int petCount = animals.Count(a => !string.IsNullOrEmpty(a.Id));
            

            if (petCount >= AppConstants.MaxPets)
            {
                _output.PrintLine(AppConstants.PetLimitReachedMessage);
                return;
            }
            
            _output.PrintLine(string.Format(AppConstants.CurrentPetsStatusFormat, petCount, AppConstants.MaxPets - petCount));
            string anotherPet = AppConstants.YesInput;

            while (anotherPet == AppConstants.YesInput && petCount < AppConstants.MaxPets)
            {
                _output.PrintLine(AppConstants.EnterSpeciesPrompt);
                var species = Console.ReadLine() ?? string.Empty;

                var id = Animal.GenerateId(species, petCount + 1);

                _output.PrintLine(string.Format(AppConstants.AgePromptFormat, id));
                var age = Console.ReadLine();

                _output.PrintLine(string.Format(AppConstants.PhysicalDescriptionPromptFormat, id));
                var physical = Console.ReadLine();

                _output.PrintLine(string.Format(AppConstants.PersonalityDescriptionPromptFormat, id));
                var personality = Console.ReadLine();

                _output.PrintLine(string.Format(AppConstants.NicknamePromptFormat, id));
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
                    _output.PrintLine($"Successfully added new {species} with ID: {result.Animal?.Id}");
                    petCount++;

                }
                else
                {
                    _output.PrintLine($"Failed to add new animal: {result.ErrorMessage}");
                }

                if (petCount < AppConstants.MaxPets)
                {
                    _output.PrintLine(AppConstants.AddAnotherPetPrompt);
                    anotherPet = Console.ReadLine()?.ToLower() == AppConstants.YesInput ? AppConstants.YesInput : AppConstants.NoInput;
                }
                else
                {
                    _output.PrintLine(AppConstants.PetLimitReachedMessage);
                }
            }
        }
    }
}