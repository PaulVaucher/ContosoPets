using ContosoPets.Application.UseCases.Animals;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class DisplayDogsWithCharacteristicCommand : IMenuCommand
    {
        private readonly IAnimalService _service;

        public DisplayDogsWithCharacteristicCommand(IAnimalService service)
        {
            _service = service;
        }

        public void Execute()
        {
            _service.DisplayDogsWithCharacteristic();
        }
    }
}