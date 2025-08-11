using Contosopets.Bootstrap;
using ContosoPets.Domain.Entities;
using ContosoPets.Infrastructure.AssemblyReferences;

namespace ContosoPets.Application.Ports
{
    public interface IAnimalRepository : IAssemblyReference
    {
        List<Animal> GetAllAnimals();
        void AddAnimal(Animal animal);
        void SaveChanges();
    }
}
