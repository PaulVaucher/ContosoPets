using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Domain.Entities;

namespace ContosoPets.Application.Services
{
    public interface IAnimalApplicationService
    {
        List<Animal> ListAll();
        AddAnimalResult AddNewAnimal(AddAnimalRequest request);
        List<Animal> GetAnimalsWithIncompleteAgeOrDescription();
        void CompleteAgesAndDescriptions(Dictionary<string, (string Age, string PhysicalDescription)> corrections);
        List<Animal> GetAnimalsWithIncompleteNicknameOrPersonality();
        void CompleteNicknamesAndPersonality(Dictionary<string, (string Nickname, string Personality)> corrections);
        Animal? GetAnimalById(string id);
        bool UpdateAnimalAge(string id, string newAge);
        bool UpdateAnimalPersonality(string id, string newPersonality);
        List<Animal> GetAnimalsWithCharacteristic(string species, string characteristic);
    }
}
