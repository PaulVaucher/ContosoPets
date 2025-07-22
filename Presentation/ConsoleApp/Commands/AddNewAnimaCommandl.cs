using ContosoPets.Application.UseCases.Animals;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class AddNewAnimalCommand : IMenuCommand
    {
        private readonly IAnimalService _service;

        public AddNewAnimalCommand(IAnimalService service)
        {
            _service = service;
        }

        public void Execute()
        {
            _service.AddNewAnimal();
        }
    }
}