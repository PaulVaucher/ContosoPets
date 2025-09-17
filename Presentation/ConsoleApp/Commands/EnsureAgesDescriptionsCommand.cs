using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Application.Ports;
using ContosoPets.Domain.Constants;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class EnsureAgesDescriptionsCommand : IMenuCommand
    {
        private readonly IAnimalService _service;
        private readonly ILinePrinter _output;

        public EnsureAgesDescriptionsCommand(IAnimalService service, ILinePrinter output)
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

            var corrections = new Dictionary<string, (string Age, string PhysicalDescription)>();
            foreach (var animal in incompleteAnimals)
            {
                if (string.IsNullOrEmpty(animal.Age) || animal.Age == "?")
                {
                    _output.PrintLine(string.Format(AppConstants.AgePromptComplete, animal.Id, animal.Species));
                    var age = _output.ReadLine() ?? string.Empty;
                    corrections[animal.Id] = (age, corrections.ContainsKey(animal.Id) ? corrections[animal.Id].PhysicalDescription : animal.PhysicalDescription);

                }
                if (string.IsNullOrEmpty(animal.PhysicalDescription) || animal.PhysicalDescription == "tbd")
                {
                    _output.PrintLine(string.Format(AppConstants.PhysicalDescriptionPromptComplete, animal.Id, animal.PhysicalDescription));
                    var physical = _output.ReadLine() ?? string.Empty;
                    var age = corrections.ContainsKey(animal.Id) ? corrections[animal.Id].Age : animal.Age;
                    corrections[animal.Id] = (age, physical);
                }
            }
            _service.CompleteAgesAndDescriptions(corrections);
            _output.PrintLine(AppConstants.AgeAndDescriptionCompleteMessage);
        }
    }    
}