using ContosoPets.Application.UseCases.Animals;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class EditAnimalAgeCommand : IMenuCommand
    {
        private readonly IAnimalService _service;

        public EditAnimalAgeCommand(IAnimalService service)
        {
            _service = service;
        }

        public void Execute()
        {
            _service.EditAnimalAge();
        }
    }
}