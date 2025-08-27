using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Domain.Constants;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class DisplayCatsWithCharacteristicCommand : IMenuCommand
    {
        private readonly IAnimalService _service;

        public DisplayCatsWithCharacteristicCommand(IAnimalService service)
        {
            _service = service;
        }

        public void Execute()
        {
            Console.WriteLine(string.Format(AppConstants.CharacteristicSearchPromptFormat, "cat"));
            var characteristic = Console.ReadLine() ?? string.Empty;

            var animals = _service.GetAnimalsWithCharacteristic("cat", characteristic);

            if (animals.Count > 0)
            {
                Console.WriteLine(string.Format(AppConstants.CharacteristicResultsFormat, "cat", characteristic));
                foreach (var animal in animals)
                {
                    Console.WriteLine();
                    animal.DisplayInfo();
                }
            }
            else
            {
                Console.WriteLine(string.Format(AppConstants.NoCharacteristicMatchFormat, "cat", characteristic));
            }
        }
    }
}