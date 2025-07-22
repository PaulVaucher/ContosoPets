using ContosoPets.Application.UseCases.Animals;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class EditAnimalPersonalityCommand : IMenuCommand
    {
        private readonly IAnimalService _service;

        public EditAnimalPersonalityCommand(IAnimalService service)
        {
            _service = service;
        }

        public void Execute()
        {
            _service.EditAnimalPersonality();
        }
    }
}