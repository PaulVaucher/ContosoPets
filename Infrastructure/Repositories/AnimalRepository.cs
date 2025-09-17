using ContosoPets.Application.Ports;
using ContosoPets.Domain.Constants;
using ContosoPets.Domain.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContosoPets.Infrastructure.Repositories
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly List<Animal> _animals;
        private static string DataFile => Path.Combine(AppContext.BaseDirectory, "Resources", "animals.json");
        private static JsonSerializerOptions SerializerOptions => new()
        {
            WriteIndented = true,
            Converters = {
                new JsonStringEnumConverter(),
                new AnimalJsonConverter()
            },
            PropertyNameCaseInsensitive = true,
        };

        public AnimalRepository()
        {
            if (File.Exists(DataFile))
            {
                string json = File.ReadAllText(DataFile);
                try
                {
                    var deserialized = JsonSerializer.Deserialize<List<Animal>>(json, SerializerOptions);
                    _animals = deserialized ?? new List<Animal>();
                }
                catch (Exception)
                {
                    _animals = new List<Animal>();
                }
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

        public int GetAnimalCount()
        {
            return _animals.Count(animal => !string.IsNullOrEmpty(animal.Id));
        }

        public void AddAnimal(Animal animal)
        {
            _animals.Add(animal);
        }

        public Animal? GetById(string id)
        {
            return _animals.FirstOrDefault(a => a.Id == id);
        }

        public void UpdateAnimal(Animal animal)
        {
            var existingIndex = _animals.FindIndex(a => a.Id == animal.Id);
            if (existingIndex >= 0)
            {
                _animals[existingIndex] = animal;
            }
        }

        public void DeleteAnimal(Animal animal)
        {
            _animals.RemoveAll(a => a.Id == animal.Id);
        }

        public List<Animal> GetAnimalsWithIncompleteAgeOrDescription()
        {
            return _animals
                .Where(a => !string.IsNullOrEmpty(a.Id) &&
                            (string.IsNullOrEmpty(a.Age) || a.Age == AppConstants.UnknownAge ||
                             string.IsNullOrEmpty(a.PhysicalDescription) || a.PhysicalDescription == AppConstants.DefaultValue))
                .ToList();
        }

        public List<Animal> GetAnimalsWithIncompleteNicknameOrPersonality()
        {
            return _animals
                .Where(a => !string.IsNullOrEmpty(a.Id) &&
                            (string.IsNullOrEmpty(a.Nickname) || a.Nickname == AppConstants.DefaultValue ||
                             string.IsNullOrEmpty(a.PersonalityDescription) || a.PersonalityDescription == AppConstants.DefaultValue))
                .ToList();
        }

        public List<Animal> GetAnimalsWithCharacteristic(string species, string characteristic)
        {
            if (string.IsNullOrEmpty(species) || string.IsNullOrEmpty(characteristic))
                return new List<Animal>();

            var lowerSpecies = species.ToLower();
            var lowerCharacteristic = characteristic.ToLower();

            return _animals
                .Where(a => a.Species.ToLower() == lowerSpecies
                && !string.IsNullOrEmpty(a.Id)
                && (a.PhysicalDescription.ToLower().Contains(lowerCharacteristic) ||
                    a.PersonalityDescription.ToLower().Contains(lowerCharacteristic)))
                .ToList();
        }

        public void SaveChanges()
        {
            try
            {
                var directory = Path.GetDirectoryName(DataFile);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                using var fs = new FileStream(DataFile, FileMode.Create, FileAccess.Write);
                using var writer = new Utf8JsonWriter(fs, new JsonWriterOptions { Indented = true });
                JsonSerializer.Serialize(writer, _animals, SerializerOptions);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format(AppConstants.AnimalOperationErrorFormat, ex.Message));
            }
        }
    }
}