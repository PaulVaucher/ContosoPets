using ContosoPets.Application.Ports;
using ContosoPets.Domain.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace ContosoPets.Infrastructure.Repositories
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly List<Animal> _animals;
        private static string DataFile => Path.Combine(AppContext.BaseDirectory, "Resources", "animals.json");
        private static JsonSerializerOptions SerializerOptions => new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
            TypeInfoResolver = new AnimalTypeResolver(),
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
            var directory = Path.GetDirectoryName(DataFile);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using var fs = new FileStream(DataFile, FileMode.Create, FileAccess.Write);
            using var writer = new Utf8JsonWriter(fs, new JsonWriterOptions { Indented = true });
            JsonSerializer.Serialize(writer, _animals, SerializerOptions);
        }
    }

    public class AnimalTypeResolver : DefaultJsonTypeInfoResolver
    {
        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

            if (type == typeof(Animal))
            {
                jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
                {
                    TypeDiscriminatorPropertyName = "Species",
                    IgnoreUnrecognizedTypeDiscriminators = true,
                    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType,
                    DerivedTypes =
                    {
                        new JsonDerivedType(typeof(Dog), "dog"),
                        new JsonDerivedType(typeof(Cat), "cat"),
                    }
                };
            }
            return jsonTypeInfo;

        }
    }
}
