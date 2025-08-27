using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Domain.Constants;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class DisplayDogsWithCharacteristicCommand : IMenuCommand
    {
        private readonly IAnimalService _service;

        public DisplayDogsWithCharacteristicCommand(IAnimalService service)
        {
            _service = service;
        }

        public void Execute()
        {
            Console.WriteLine(string.Format(AppConstants.CharacteristicSearchPromptFormat, "dog"));
            var characteristic = Console.ReadLine() ?? string.Empty;

            var animals = _service.GetAnimalsWithCharacteristic("dog", characteristic);

            if (animals.Count > 0)
            {
                Console.WriteLine(string.Format(AppConstants.CharacteristicResultsFormat, "dog", characteristic));
                foreach (var animal in animals)
                {
                    Console.WriteLine();
                    animal.DisplayInfo();
                }
            }
            else
            {
                Console.WriteLine(string.Format(AppConstants.NoCharacteristicMatchFormat, "dog", characteristic));
            }
        }
    }
}