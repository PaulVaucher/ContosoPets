using ContosoPets.Application.UseCases.Animals;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class EnsureNicknamesPersonalityCommand : IMenuCommand
    {
        private readonly IAnimalService _service;

        public EnsureNicknamesPersonalityCommand(IAnimalService service)
        {
            _service = service;
        }

        public void Execute()
        {
            _service.EnsureNicknamesAndPersonalityComplete();
        }
    }
}