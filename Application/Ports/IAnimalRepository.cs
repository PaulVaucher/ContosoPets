using ContosoPets.Domain.Entities;
using ContosoPets.Infrastructure.AssemblyReferences;

namespace ContosoPets.Application.Ports
{
    public interface IAnimalRepository : IAssemblyReference
    {
        List<Animal> GetAllAnimals();
        void AddAnimal(Animal animal);
        Animal? GetById(string id);
        void UpdateAnimal(Animal animal);
        void DeleteAnimal(Animal animal);
        void SaveChanges(); // for compatibility with interface
    }
}
