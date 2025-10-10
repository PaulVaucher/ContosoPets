using System.Text.Json;
using System.Text.Json.Serialization;
using ContosoPets.Domain.Entities;

namespace ContosoPets.Infrastructure.Serialization
{
    public class AnimalJsonConverter : JsonConverter<Animal>
    {
        public override Animal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            try
            {
                // Lire toutes les propriétés manuellement
                string species = root.GetProperty("Species").GetString()?.ToLower() ?? "";
                string id = root.GetProperty("Id").GetString() ?? "";
                string age = root.GetProperty("Age").GetString() ?? "?";
                string physicalDescription = root.GetProperty("PhysicalDescription").GetString() ?? "tbd";
                string personalityDescription = root.GetProperty("PersonalityDescription").GetString() ?? "tbd";
                string nickname = root.GetProperty("Nickname").GetString() ?? "tbd";

                // Créer directement avec les constructeurs (paramètres en minuscules)
                return species switch
                {
                    "dog" => new Dog(species, id, age, physicalDescription, personalityDescription, nickname),
                    "cat" => new Cat(species, id, age, physicalDescription, personalityDescription, nickname),
                    _ => throw new JsonException($"Unknown species: {species}")
                };
            }
            catch (Exception ex)
            {
                throw new JsonException($"Error deserializing Animal: {ex.Message}", ex);
            }
        }

        public override void Write(Utf8JsonWriter writer, Animal value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Species", value.Species);
            writer.WriteString("Id", value.Id.Value); // ✅ Utiliser .Value pour AnimalId
            writer.WriteString("Age", value.Age);
            writer.WriteString("PhysicalDescription", value.PhysicalDescription);
            writer.WriteString("PersonalityDescription", value.PersonalityDescription);
            writer.WriteString("Nickname", value.Nickname);
            writer.WriteEndObject();
        }
    }
}