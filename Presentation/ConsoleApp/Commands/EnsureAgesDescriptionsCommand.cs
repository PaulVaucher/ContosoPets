using ContosoPets.Application.UseCases.Animals;

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
            _service.EnsureAgesAndDescriptionsComplete();
        }
    }
}