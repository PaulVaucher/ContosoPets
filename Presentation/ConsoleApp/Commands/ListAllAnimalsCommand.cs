using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Application.Ports;
using ContosoPets.Domain.Constants;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class ListAllAnimalsCommand : IMenuCommand
    {
        private readonly IAnimalService _service;
        private readonly ILinePrinter _output;

        public ListAllAnimalsCommand(IAnimalService service, ILinePrinter output)
        {
            _service = service;
            _output = output;
        }

        public void Execute()
        {
            var animals = _service.ListAll();
            if (animals.Count == 0)
            {
                _output.PrintLine(AppConstants.NoAnimalsFoundMessage);
            }
            else
            {
                foreach (var animal in animals)
                {
                    animal.DisplayInfo(_output);
                    _output.PrintLine();
                }
            }
        }
    }
}
