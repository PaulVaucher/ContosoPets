using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Application.Ports;
using ContosoPets.Domain.Constants;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class DisplayDogsWithCharacteristicCommand : IMenuCommand
    {
        private readonly IAnimalService _service;
        private readonly ILinePrinter _output;

        public DisplayDogsWithCharacteristicCommand(IAnimalService service, ILinePrinter output)
        {
            _service = service;
            _output = output;
        }

        public void Execute()
        {
            _output.PrintLine(string.Format(AppConstants.CharacteristicSearchPromptFormat, "dog"));
            var characteristic = Console.ReadLine() ?? string.Empty;

            var animals = _service.GetAnimalsWithCharacteristic("dog", characteristic);

            if (animals.Count > 0)
            {
                _output.PrintLine(string.Format(AppConstants.CharacteristicResultsFormat, "dog", characteristic));
                foreach (var animal in animals)
                {
                    _output.PrintLine();
                    animal.DisplayInfo(_output);
                }
            }
            else
            {
                _output.PrintLine(string.Format(AppConstants.NoCharacteristicMatchFormat, "dog", characteristic));
            }
        }
    }
}