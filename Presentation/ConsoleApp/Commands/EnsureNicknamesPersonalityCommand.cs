using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Domain.Constants;

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
            var incompleteAnimals = _service.GetAnimalsWithIncompleteNicknameOrPersonality();

            if (incompleteAnimals.Count == 0)
            {
                Console.WriteLine(AppConstants.NoAnimalsFoundMessage);
                return;
            }

            var corrections = new Dictionary<string, (string Nickname, string PersonalityDescription)>();

            foreach (var animal in incompleteAnimals)
            {
                string nickname = animal.Nickname;
                string personality = animal.PersonalityDescription;

                if (string.IsNullOrEmpty(animal.Nickname) || animal.Nickname == "tbd")
                {
                    Console.WriteLine(string.Format(AppConstants.NicknamePromptFormat, animal.Id));
                    nickname = Console.ReadLine() ?? string.Empty;
                }
                if (string.IsNullOrEmpty(animal.PersonalityDescription) || animal.PersonalityDescription == "tbd")
                {
                    Console.WriteLine(string.Format(AppConstants.PersonalityDescriptionPromptFormat, animal.Id));
                    personality = Console.ReadLine() ?? string.Empty;
                }
                corrections[animal.Id] = (nickname, personality);
            }
            _service.CompleteNicknamesAndPersonality(corrections);
            Console.WriteLine(AppConstants.NicknamePersonalityCompleteMessage);
        }
    }
}