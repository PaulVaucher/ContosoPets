using ContosoPets.Application.Services;
using ContosoPets.Application.Ports;
using ContosoPets.Domain.Constants;
using ContosoPets.Domain.ValueObjects;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class EnsureAgesDescriptionsCommand : IMenuCommand
    {
        private readonly IAnimalApplicationService _service;
        private readonly ILinePrinter _output;

        public EnsureAgesDescriptionsCommand(IAnimalApplicationService service, ILinePrinter output)
        {
            _service = service;
            _output = output;
        }

        public void Execute()
        {
            var incompleteAnimals = _service.GetAnimalsWithIncompleteAgeOrDescription();
            if (incompleteAnimals.Count == 0)
            {
                _output.PrintLine(AppConstants.NoAnimalsFoundMessage);
                return;
            }

            var corrections = new Dictionary<AnimalId, (string Age, string PhysicalDescription)>();
            foreach (var animal in incompleteAnimals)
            {
                string age = animal.Age;
                string physical = animal.PhysicalDescription;

                if (string.IsNullOrEmpty(animal.Age) || animal.Age == "?")
                {
                    _output.PrintLine(string.Format(AppConstants.AgePromptComplete, animal.Id, animal.Species));
                    age = _output.ReadLine() ?? string.Empty;

                }
                if (string.IsNullOrEmpty(animal.PhysicalDescription) || animal.PhysicalDescription == "tbd")
                {
                    _output.PrintLine(string.Format(AppConstants.PhysicalDescriptionPromptComplete, animal.Id, animal.PhysicalDescription));
                    physical = _output.ReadLine() ?? string.Empty;                    
                }
                corrections[animal.Id] = (age, physical);
            }
            _service.CompleteAgesAndDescriptions(corrections);
            _output.PrintLine(AppConstants.AgeAndDescriptionCompleteMessage);
        }
    }    
}