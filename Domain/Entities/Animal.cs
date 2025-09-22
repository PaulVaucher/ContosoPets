using ContosoPets.Application.Ports;
using ContosoPets.Domain.ValueObjects;

namespace ContosoPets.Domain.Entities
{
    public abstract class Animal
    {
        public string Species { get; private set; } = string.Empty;
        public AnimalId Id { get; private set; } = null!;
        public string Age { get; private set; } = "?";
        public string PhysicalDescription { get; private set; } = "tbd";
        public string PersonalityDescription { get; private set; } = "tbd";
        public string Nickname { get; private set; } = "tbd";

        protected Animal() { }

        protected Animal(string species, string id, string age, string physicalDescription, string personalityDescription, string nickname)
        {
            Species = species;
            Id = new AnimalId(id);
            Age = age;
            PhysicalDescription = physicalDescription;
            PersonalityDescription = personalityDescription;
            Nickname = nickname;
        }

        public virtual void SetAge(string value) => Age = string.IsNullOrEmpty(value) ? "?" : value;

        public virtual void SetPhysicalDescription(string value) =>
            PhysicalDescription = string.IsNullOrEmpty(value) ? "tbd" : value;

        public virtual void SetPersonalityDescription(string value) =>
            PersonalityDescription = string.IsNullOrEmpty(value) ? "tbd" : value;

        public virtual void SetNickname(string value) =>
            Nickname = string.IsNullOrEmpty(value) ? "tbd" : value;

        public virtual void DisplayInfo(ILinePrinter output)
        {
            output.PrintLine($"ID: {Id}");
            output.PrintLine($"Species: {Species}");
            output.PrintLine($"Age: {(string.IsNullOrEmpty(Age) ? "?" : Age)}");
            output.PrintLine($"Physical Description: {PhysicalDescription}");
            output.PrintLine($"Personality Description: {PersonalityDescription}");
            output.PrintLine($"Nickname: {Nickname}");
        }

        public static string GenerateId(string species, int index)
        {
            string prefix = species.ToLower() switch
            {
                "dog" => "d",
                "cat" => "c",
                _ => "u"
            };

            return $"{prefix}{index}";
        }
    }
}
