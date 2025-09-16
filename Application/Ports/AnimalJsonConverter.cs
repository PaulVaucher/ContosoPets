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

                // Read the Species property to determine the type
                if (root.TryGetProperty("Species", out var speciesProperty))
                {
                    string species = speciesProperty.GetString()?.ToLower() ?? "";

                    // Create options without this converter to avoid recursion
                    var newOptions = new JsonSerializerOptions(options);
                    newOptions.Converters.Clear();
                    foreach (var converter in options.Converters)
                    {
                        if (converter != this)
                        {
                            newOptions.Converters.Add(converter);
                        }
                    }

                    // Deserialize to the appropriate type
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
            // Create options without this converter to avoid recursion
            var newOptions = new JsonSerializerOptions(options);
            newOptions.Converters.Clear();
            foreach (var converter in options.Converters)
            {
                if (converter != this)
                {
                    newOptions.Converters.Add(converter);
                }
            }

            // Serialize normally
            JsonSerializer.Serialize(writer, value, value.GetType(), newOptions);
        }
    }
}