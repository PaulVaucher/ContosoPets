using ContosoPets.Application.Ports;
using ContosoPets.Domain.Entities;
using System.Text.Json;

namespace ContosoPets.Infrastructure.Repositories
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly List<Animal> _animals;
        private static string DataFile => Path.Combine(AppContext.BaseDirectory, "Resources", "animals.json");

        public AnimalRepository()
        {
            if (File.Exists(DataFile))
            {
                string json = File.ReadAllText(DataFile);
                var deserialilized = JsonSerializer.Deserialize<List<Animal>>(json);
                _animals = deserialilized ?? new List<Animal>();
            }
            else
            {
                _animals = new List<Animal>();
            }
        }

        public List<Animal> GetAllAnimals()
        {
            return _animals;
        }

        public void AddAnimal(Animal animal)
        {
            _animals.Add(animal);
        }

        public void SaveChanges()
        {
            var json = JsonSerializer.Serialize(_animals, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(DataFile, json);
        }
    }
}
