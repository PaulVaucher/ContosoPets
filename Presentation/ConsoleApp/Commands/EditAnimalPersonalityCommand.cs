using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Domain.Constants;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class EditAnimalPersonalityCommand : IMenuCommand
    {
        private readonly IAnimalService _service;

        public EditAnimalPersonalityCommand(IAnimalService service)
        {
            _service = service;
        }

        public void Execute()
        {
            Console.WriteLine(string.Format(AppConstants.EnterAnimalIdPrompt, "personality"));
            var id = Console.ReadLine() ?? string.Empty;

            var animal = _service.GetAnimalById(id);

            if (animal == null)
            {
                Console.WriteLine(AppConstants.AnimalNotFoundMessage);
                return;
            }

            Console.WriteLine(string.Format(AppConstants.CurrentPersonalityFormat, id, animal.PersonalityDescription));
            Console.WriteLine(string.Format(AppConstants.PersonalityDescriptionPromptFormat, id));
            var newPersonality = Console.ReadLine() ?? string.Empty;

            if (_service.UpdateAnimalPersonality(id, newPersonality))
                Console.WriteLine(AppConstants.UpdatedPersonalityMessage);
            else
                Console.WriteLine(AppConstants.AnimalNotFoundMessage);
        }
    }
}