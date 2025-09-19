using ContosoPets.Application.Services;
using ContosoPets.Application.Ports;
using ContosoPets.Domain.Constants;
using ContosoPets.Domain.ValueObjects;

namespace ContosoPets.Presentation.ConsoleApp.Commands
{
    public class EnsureNicknamesPersonalityCommand : IMenuCommand
    {
        private readonly IAnimalApplicationService _service;
        private readonly ILinePrinter _output;

        public EnsureNicknamesPersonalityCommand(IAnimalApplicationService service, ILinePrinter output)
        {
            _service = service;
            _output = output;

        }

        public void Execute()
        {
            var incompleteAnimals = _service.GetAnimalsWithIncompleteNicknameOrPersonality();

            if (incompleteAnimals.Count == 0)
            {
                _output.PrintLine(AppConstants.NoAnimalsFoundMessage);
                return;
            }

            var corrections = new Dictionary<AnimalId, (string Nickname, string PersonalityDescription)>();

            foreach (var animal in incompleteAnimals)
            {
                string nickname = animal.Nickname;
                string personality = animal.PersonalityDescription;

                if (string.IsNullOrEmpty(animal.Nickname) || animal.Nickname == "tbd")
                {
                    _output.PrintLine(string.Format(AppConstants.NicknamePromptFormat, animal.Id));
                    nickname = _output.ReadLine() ?? string.Empty;
                }
                if (string.IsNullOrEmpty(animal.PersonalityDescription) || animal.PersonalityDescription == "tbd")
                {
                    _output.PrintLine(string.Format(AppConstants.PersonalityDescriptionPromptFormat, animal.Id));
                    personality = _output.ReadLine() ?? string.Empty;                    
                }
                corrections[animal.Id] = (nickname, personality);
            }
            _service.CompleteNicknamesAndPersonality(corrections);
            _output.PrintLine(AppConstants.NicknamePersonalityCompleteMessage);
        }
    }
}