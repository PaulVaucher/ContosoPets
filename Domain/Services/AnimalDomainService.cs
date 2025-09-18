using ContosoPets.Domain.Builders;
using ContosoPets.Domain.Constants;
using ContosoPets.Domain.Entities;

namespace ContosoPets.Domain.Services
{
    public class AnimalDomainService : IAnimalDomainService
    {
        private static readonly HashSet<string> SupportedSpecies = new(StringComparer.OrdinalIgnoreCase)
        {
            "dog",
            "cat"
        };
        
        public string? ValidateNewAnimal(string species, int petCount)
        {
            if (petCount >= AppConstants.MaxPets)
            {
                return AppConstants.PetLimitReachedMessage;
            }
            if (string.IsNullOrWhiteSpace(species) || !IsSupportedSpecies(species))
            {
                return AppConstants.InvalidSpeciesMessage;
            }
            return null;
        }
        public string GenerateId(string species, int nextIndex) =>
            Animal.GenerateId(species, nextIndex);
        public Animal BuildAnimal(string species, string id, string age, string physicalDescription,
            string personalityDescription, string nickname)
        {
            return AnimalBuilder.Builder()
                .WithSpecies(species)
                .WithId(id)
                .WithAge(age)
                .WithPhysicalDescription(physicalDescription)
                .WithPersonalityDescription(personalityDescription)
                .WithNickname(nickname)
                .Build();
        }
        public bool IsSupportedSpecies(string species) =>
            !string.IsNullOrWhiteSpace(species) && SupportedSpecies.Contains(species);
    }
}
