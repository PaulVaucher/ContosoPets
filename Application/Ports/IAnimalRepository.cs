using ContosoPets.Domain.Entities;
using ContosoPets.Infrastructure.AssemblyReferences;

namespace ContosoPets.Application.Ports
{
    public interface IAnimalRepository : IAssemblyReference
    {
        List<Animal> GetAllAnimals();
        int GetAnimalCount();
        void AddAnimal(Animal animal);
        Animal? GetById(string id);
        void UpdateAnimal(Animal animal);
        void DeleteAnimal(Animal animal);
        List<Animal> GetAnimalsWithIncompleteAgeOrDescription();
        List<Animal> GetAnimalsWithIncompleteNicknameOrPersonality();
        List<Animal> GetAnimalsWithCharacteristic(string species, string characteristic);
        void SaveChanges(); // for compatibility with interface
    }
}
