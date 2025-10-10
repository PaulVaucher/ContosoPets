using ContosoPets.Application.Ports;
using ContosoPets.Application.Services;
using ContosoPets.Domain.Constants;
using ContosoPets.Presentation.ConsoleApp.Commands;
using Microsoft.Extensions.Logging;

namespace ContosoPets.Presentation.ConsoleApp.UI
{
    public class MenuHandler
    {
        private readonly ILogger<object> _logger;
        private readonly IAnimalApplicationService _service;
        private readonly ILinePrinter _output;

        public MenuHandler(ILogger<MenuHandler> logger, IAnimalApplicationService service, ILinePrinter output)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _output = output ?? throw new ArgumentNullException(nameof(output));
        }

        public void RunInteractiveMenu()
        {
            _output.PrintLine(AppConstants.WelcomeMessage);

            bool exit = false;
            void ExitApp() => exit = true;

            var (orderedMenu, commandMap) = CommandRegistry.BuildCommandRegistry(_service, _output, ExitApp);

            while (!exit)
            {
                try
                {
                    DisplayMenu(orderedMenu);
                    ProcessUserInput(commandMap);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, LoggingConstants.UnexpectedError, nameof(MenuHandler), nameof(RunInteractiveMenu));
                    _output.PrintLine(string.Format(AppConstants.MenuExecutionErrorFormat, ex.Message));
                    _output.PrintLine(AppConstants.ContinuePrompt);
                    _output.ReadKey();
                    _output.Clear();
                }
            }
        }

        private void DisplayMenu(List<MenuCommandEntry> orderedMenu)
        {
            _output.PrintLine();
            foreach (var entry in orderedMenu)
            {
                _output.PrintLine(entry.Option.ToLabel());
            }
            _output.Write(AppConstants.MenuPrompt);
        }

        private void ProcessUserInput(Dictionary<MenuOptionEnum, IMenuCommand> commandMap)
        {
            var input = _output.ReadLine();

            _logger?.LogDebug(LoggingConstants.UserInputReceived, LoggingConstants.InputTypeMenuSelection, input ?? LoggingConstants.InputValueNull);

            if (int.TryParse(input, out int menuChoice) &&
                Enum.IsDefined(typeof(MenuOptionEnum), menuChoice))
            {
                var selected = (MenuOptionEnum)menuChoice;
                _output.PrintLine();

                _logger?.LogInformation(LoggingConstants.UserMenuSelection, selected);

                if (commandMap.TryGetValue(selected, out var command))
                {
                    ExecuteCommand(command);
                }
                else
                {
                    _output.PrintLine(AppConstants.InvalidOptionMessage);
                }
            }
            else
            {
                _logger?.LogWarning(LoggingConstants.InvalidUserInput, LoggingConstants.InputTypeMenuSelection, input ?? LoggingConstants.InputValueNull);
                _output.PrintLine(AppConstants.InvalidOptionMessage);
            }
        }

        private void ExecuteCommand(IMenuCommand command)
        {
            try
            {
                using var scope = _logger?.BeginScope("Command={CommandType}", command.GetType().Name);
                _logger?.LogDebug(LoggingConstants.ExecutingCommand, command.GetType().Name);

                command.Execute();

                _logger?.LogDebug(LoggingConstants.CommandExecutedSuccessfully, command.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, LoggingConstants.CommandExecutionFailed, command.GetType().Name);
                _output.PrintLine(string.Format(AppConstants.MenuExecutionErrorFormat, ex.Message));
                _output.PrintLine(AppConstants.ContinuePrompt);
                _output.ReadKey();
            }
        }
    }
}
