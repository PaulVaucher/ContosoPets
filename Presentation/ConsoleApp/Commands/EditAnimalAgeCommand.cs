using ContosoPets.Application.Services;
using ContosoPets.Application.Ports;
using ContosoPets.Domain.Constants;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class EditAnimalAgeCommand : IMenuCommand
    {
        private readonly IAnimalApplicationService _service;
        private readonly ILinePrinter _output;

        public EditAnimalAgeCommand(IAnimalApplicationService service, ILinePrinter output)
        {
            _service = service;
            _output = output;
        }

        public void Execute()
        {
            _output.PrintLine(string.Format(AppConstants.EnterAnimalIdPrompt, "age"));
            var id = _output.ReadLine() ?? string.Empty;
            var animal = _service.GetAnimalById(id);

            if (animal == null)
            {
                _output.PrintLine(AppConstants.AnimalNotFoundMessage);
                return;
            }

            _output.PrintLine(string.Format(AppConstants.CurrentAgeFormat, id, animal.Age));
            var newAge = _output.ReadLine() ?? string.Empty;

            if (_service.UpdateAnimalAge(id, newAge))
                _output.PrintLine(string.Format(AppConstants.UpdatedAgeFormat, id, newAge));
            else
                _output.PrintLine(AppConstants.AnimalNotFoundMessage);
        }
    }
}