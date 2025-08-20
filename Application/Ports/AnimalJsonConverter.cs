using System.Text.Json;
using System.Text.Json.Serialization;
using ContosoPets.Domain.Entities;

namespace ContosoPets.Application.Ports
{
    public class AnimalJsonConverter : JsonConverter<Animal>
    {
        public override Animal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

                // Lire la propriété Species pour déterminer le type
                if (root.TryGetProperty("Species", out var speciesProperty))
                {
                    string species = speciesProperty.GetString()?.ToLower() ?? "";

                    // Créer les options sans ce convertisseur pour éviter la récursion
                    var newOptions = new JsonSerializerOptions(options);
                    newOptions.Converters.Clear();
                    foreach (var converter in options.Converters)
                    {
                        if (converter != this)
                        {
                            newOptions.Converters.Add(converter);
                        }
                    }

                    // Désérialiser vers le type approprié
                    string json = root.GetRawText();
                    return species switch
                    {
                        "dog" => JsonSerializer.Deserialize<Dog>(json, newOptions),
                        "cat" => JsonSerializer.Deserialize<Cat>(json, newOptions),
                        _ => throw new JsonException($"Unknown species: {species}")
                    };
                }

                throw new JsonException("Species property not found");
            
        }

        public override void Write(Utf8JsonWriter writer, Animal value, JsonSerializerOptions options)
        {
            // Créer les options sans ce convertisseur pour éviter la récursion
            var newOptions = new JsonSerializerOptions(options);
            newOptions.Converters.Clear();
            foreach (var converter in options.Converters)
            {
                if (converter != this)
                {
                    newOptions.Converters.Add(converter);
                }
            }

            // Sérialiser normalement
            JsonSerializer.Serialize(writer, value, value.GetType(), newOptions);
        }
    }
}