using ContosoPets.Application.Services;
using ContosoPets.Application.Ports;
using ContosoPets.Domain.Constants;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class DisplayCatsWithCharacteristicCommand : IMenuCommand
    {
        private readonly IAnimalApplicationService _service;
        private readonly ILinePrinter _output;

        public DisplayCatsWithCharacteristicCommand(IAnimalApplicationService service, ILinePrinter output)
        {
            _service = service;
            _output = output;
        }

        public void Execute()
        {
            _output.PrintLine(string.Format(AppConstants.CharacteristicSearchPromptFormat, "cat"));
            var characteristic = Console.ReadLine() ?? string.Empty;

            var animals = _service.GetAnimalsWithCharacteristic("cat", characteristic);

            if (animals.Count > 0)
            {
                _output.PrintLine(string.Format(AppConstants.CharacteristicResultsFormat, "cat", characteristic));
                foreach (var animal in animals)
                {
                    _output.PrintLine();
                    animal.DisplayInfo(_output);
                }
            }
            else
            {
                _output.PrintLine(string.Format(AppConstants.NoCharacteristicMatchFormat, "cat", characteristic));
            }
        }
    }
}