using ContosoPets.Application.Ports;
using ContosoPets.Domain.Constants;
using ContosoPets.Domain.Entities;

namespace ContosoPets.UnitTests.TestInfrastructure.Fakes
{
    public class FakeAnimalRepository : IAnimalRepository
    {
        private readonly List<Animal> _animals = new();

        public List<Animal> GetAllAnimals() => _animals.ToList();
        public int GetAnimalCount() => _animals.Count;

        public void AddAnimal(Animal animal)
        {
            if (_animals.Any(a => a.Id.Value == animal.Id. Value))
            {
                throw new InvalidOperationException($"Animal with ID {animal.Id.Value} already exists.");
            }
            _animals.Add(animal);
        }

        public Animal? GetById(string id) => _animals.FirstOrDefault(a => a.Id.Value == id);

        public void UpdateAnimal(Animal animal)
        {
            var existingAnimal = GetById(animal.Id.Value);
            if (existingAnimal != null)
            {
                _animals.Remove(existingAnimal);
                _animals.Add(animal);
            }
        }

        public void DeleteAnimal(Animal animal) =>
            _animals.RemoveAll(a => a.Id.Value == animal.Id.Value);

        public List<Animal> GetAnimalsWithIncompleteAgeOrDescription() =>
            _animals.Where(a =>
                string.IsNullOrWhiteSpace(a.Age) || a.Age == AppConstants.UnknownAge ||
                string.IsNullOrWhiteSpace(a.PhysicalDescription) || a.PhysicalDescription == AppConstants.DefaultValue
            ).ToList();

        public List<Animal> GetAnimalsWithIncompleteNicknameOrPersonality() =>
            _animals.Where(a =>
                string.IsNullOrWhiteSpace(a.Nickname) || a.Nickname == AppConstants.DefaultValue ||
                string.IsNullOrWhiteSpace(a.PersonalityDescription) || a.PersonalityDescription == AppConstants.DefaultValue
            ).ToList();

        public List<Animal> GetAnimalsWithCharacteristic(string species, string characteristic)
        {
            var lowerSpecies = species.ToLower();
            var lowerCharacteristic = characteristic.ToLower();
            return _animals.Where(a =>
                a.Species.ToLower() == lowerSpecies &&
                (a.PhysicalDescription.ToLower().Contains(lowerCharacteristic) ||
                 a.PersonalityDescription.ToLower().Contains(lowerCharacteristic))
            ).ToList();
        }

        public void Clear() => _animals.Clear();
        public void SeedWith(params Animal[] animals) => _animals.AddRange(animals);
    }
}
