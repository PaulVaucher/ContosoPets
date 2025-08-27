using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Domain.Constants;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class EditAnimalAgeCommand : IMenuCommand
    {
        private readonly IAnimalService _service;

        public EditAnimalAgeCommand(IAnimalService service)
        {
            _service = service;
        }

        public void Execute()
        {
            Console.WriteLine(AppConstants.EnterAnimalIdPrompt, "age");
            var id = Console.ReadLine() ?? string.Empty;
            var animal = _service.GetAnimalById(id);

            if (animal == null)
            {
                Console.WriteLine(AppConstants.AnimalNotFoundMessage);
                return;
            }

            Console.WriteLine(string.Format(AppConstants.CurrentAgeFormat, id, animal.Age));
            Console.WriteLine(string.Format(AppConstants.AgePromptFormat, id));
            var newAge = Console.ReadLine() ?? string.Empty;

            if (_service.UpdateAnimalAge(id, newAge))
                Console.WriteLine(string.Format(AppConstants.UpdatedAgeFormat, id, newAge));
            else
                Console.WriteLine(AppConstants.AnimalNotFoundMessage);
        }
    }
}