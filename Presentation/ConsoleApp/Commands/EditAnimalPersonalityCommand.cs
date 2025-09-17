using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Application.Ports;
using ContosoPets.Domain.Constants;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class EditAnimalPersonalityCommand : IMenuCommand
    {
        private readonly IAnimalService _service;
        private readonly ILinePrinter _output;

        public EditAnimalPersonalityCommand(IAnimalService service, ILinePrinter output)
        {
            _service = service;
            _output = output;
        }

        public void Execute()
        {
            _output.PrintLine(string.Format(AppConstants.EnterAnimalIdPrompt, "personality"));
            var id = _output.ReadLine() ?? string.Empty;

            var animal = _service.GetAnimalById(id);

            if (animal == null)
            {
                _output.PrintLine(AppConstants.AnimalNotFoundMessage);
                return;
            }

            _output.PrintLine(string.Format(AppConstants.CurrentPersonalityFormat, id, animal.PersonalityDescription));
            _output.PrintLine(string.Format(AppConstants.PersonalityDescriptionPromptFormat, id));
            var newPersonality = _output.ReadLine() ?? string.Empty;

            if (_service.UpdateAnimalPersonality(id, newPersonality))
                _output.PrintLine(AppConstants.UpdatedPersonalityMessage);
            else
                _output.PrintLine(AppConstants.AnimalNotFoundMessage);
        }
    }
}