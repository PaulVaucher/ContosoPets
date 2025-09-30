using ContosoPets.Domain.Constants;
using ContosoPets.Domain.Entities;
using ContosoPets.Domain.Services;

namespace ContosoPets.UnitTests.TestInfrastructure.Fakes
{
    internal class FakeAnimalDomainService : IAnimalDomainService
    {
        public string? ValidateNewAnimal(string species, int petCount)
        {
            if (!IsSupportedSpecies(species))
            {
                return AppConstants.InvalidSpeciesMessage;
            }

            if (petCount >= AppConstants.MaxPets)
            {
                return AppConstants.PetLimitReachedMessage;
            }
            return null;
        }

        public string GenerateId(string species, int nextIndex)
        {
            return Animal.GenerateId(species, nextIndex);
        }

        public Animal BuildAnimal(string species, string id, string age, string physicalDescription,
            string personalityDescription, string nickname)
        {
            return species.ToLower() switch
            {
                "dog" => new Dog(species, id, age, physicalDescription, personalityDescription, nickname),
                "cat" => new Cat(species, id, age, physicalDescription, personalityDescription, nickname),
                _ => throw new ArgumentException($"Unsupported species: {species}")
            };
        }

        public bool IsSupportedSpecies(string species) => species?.ToLower() is "dog" or "cat";
    }
}
