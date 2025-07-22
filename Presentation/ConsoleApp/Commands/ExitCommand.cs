using ContosoPets.Domain.Constants;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class ExitCommand : IMenuCommand
    {
        private readonly Action _onExit;

        public ExitCommand(Action onExit)
        {
            _onExit = onExit;
        }

        public void Execute()
        {
            Console.WriteLine(AppConstants.GoodbyeMessage);
            _onExit?.Invoke();
        }
    }
}
