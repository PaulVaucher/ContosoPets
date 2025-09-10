using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Domain.Constants;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class EnsureAgesDescriptionsCommand : IMenuCommand
    {
        private readonly IAnimalService _service;

        public EnsureAgesDescriptionsCommand(IAnimalService service)
        {
            _service = service;
        }

        public void Execute()
        {
            var incompleteAnimals = _service.GetAnimalsWithIncompleteAgeOrDescription();
            if (incompleteAnimals.Count == 0)
            {
                Console.WriteLine(AppConstants.NoAnimalsFoundMessage);
                return;
            }

            var corrections = new Dictionary<string, (string Age, string PhysicalDescription)>();
            foreach (var animal in incompleteAnimals)
            {
                if (string.IsNullOrEmpty(animal.Age) || animal.Age == "?")
                {
                    Console.WriteLine(string.Format(AppConstants.AgePromptComplete, animal.Id, animal.Species));
                    var age = Console.ReadLine() ?? string.Empty;
                    corrections[animal.Id] = (age, corrections.ContainsKey(animal.Id) ? corrections[animal.Id].PhysicalDescription : animal.PhysicalDescription);

                }
                if (string.IsNullOrEmpty(animal.PhysicalDescription) || animal.PhysicalDescription == "tbd")
                {
                    Console.WriteLine(string.Format(AppConstants.PhysicalDescriptionPromptComplete, animal.Id, animal.PhysicalDescription));
                    var physical = Console.ReadLine() ?? string.Empty;
                    var age = corrections.ContainsKey(animal.Id) ? corrections[animal.Id].Age : animal.Age;
                    corrections[animal.Id] = (age, physical);
                }
            }
            _service.CompleteAgesAndDescriptions(corrections);
            Console.WriteLine(AppConstants.AgeAndDescriptionCompleteMessage);
        }
    }
}