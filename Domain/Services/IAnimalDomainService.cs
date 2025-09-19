using ContosoPets.Domain.Entities;

namespace ContosoPets.Domain.Services
{    
    public interface IAnimalDomainService
    {
        string? ValidateNewAnimal(string species, int petCount);
        string GenerateId(string species, int nextIndex);
        Animal BuildAnimal(string species, string id, string age, string physicalDescription,
            string personalityDescription, string nickname);
        bool IsSupportedSpecies(string species);
    }
}