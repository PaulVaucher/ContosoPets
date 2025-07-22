using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Domain.Constants;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class ListAllAnimalsCommand : IMenuCommand
    {
        private readonly IAnimalService _service;

        public ListAllAnimalsCommand(IAnimalService service)
        {
            _service = service;
        }

        public void Execute()
        {
            var animals = _service.ListAll();
            if (animals.Count == 0)
            {
                Console.WriteLine(AppConstants.NoAnimalsFoundMessage);
            }
            else
            {
                foreach (var animal in animals)
                {
                    animal.DisplayInfo();
                    Console.WriteLine();
                }
            }
        }
    }
}
