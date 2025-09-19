using ContosoPets.Application.Ports;
using ContosoPets.Domain.Constants;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class ExitCommand : IMenuCommand
    {
        private readonly ILinePrinter _output;
        private readonly Action _onExit;

        public ExitCommand(ILinePrinter output, Action onExit)
        {
            _output = output;
            _onExit = onExit;
        }

        public void Execute()
        {
            _output.PrintLine(AppConstants.GoodbyeMessage);
            _onExit?.Invoke();
        }
    }
}
