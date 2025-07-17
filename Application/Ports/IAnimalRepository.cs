using ContosoPets.Domain.Entities;

namespace ContosoPets.Application.Ports
{
    public interface IAnimalRepository
    {
        List<Animal> GetAllAnimals();
        void AddAnimal(Animal animal);
        void SaveChanges();
    }
}
