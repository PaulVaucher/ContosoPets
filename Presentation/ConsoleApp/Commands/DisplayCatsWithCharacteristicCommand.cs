using ContosoPets.Application.UseCases.Animals;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class DisplayCatsWithCharacteristicCommand : IMenuCommand
    {
        private readonly IAnimalService _service;

        public DisplayCatsWithCharacteristicCommand(IAnimalService service)
        {
            _service = service;
        }

        public void Execute()
        {
            _service.DisplayCatsWithCharacteristic();
        }
    }
}