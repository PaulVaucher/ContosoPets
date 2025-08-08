using ContosoPets.Application.Ports;
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

        public void AddAnimal(Animal animal)
        {
            _animals.Add(animal);
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
                Console.WriteLine($"Save error: {ex.Message}");
            }
        }
    }
}
