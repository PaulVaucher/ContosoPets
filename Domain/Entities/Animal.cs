namespace ContosoPets.Domain.Entities
{
    public abstract class Animal
    {
        public string Species { get; private set; } = string.Empty;
        public string Id { get; private set; } = string.Empty;
        public string Age { get; private set; } = "?";
        public string PhysicalDescription { get; private set; } = "tbd";
        public string PersonalityDescription { get; private set; } = "tbd";
        public string Nickname { get; private set; } = "tbd";

        protected Animal() { }

        protected Animal(string species, string id, string age, string physicalDescription, string personalityDescription, string nickname)
        {
            Species = species;
            Id = id;
            Age = age;
            PhysicalDescription = physicalDescription;
            PersonalityDescription = personalityDescription;
            Nickname = nickname;
        }

        public void SetAge(string value) => Age = string.IsNullOrEmpty(value) ? "?" : value;

        public void SetPhysicalDescription(string value) =>
            PhysicalDescription = string.IsNullOrEmpty(value) ? "tbd" : value;

        public void SetPersonalityDescription(string value) =>
            PersonalityDescription = string.IsNullOrEmpty(value) ? "tbd" : value;

        public void SetNickname(string value) =>
            Nickname = string.IsNullOrEmpty(value) ? "tbd" : value;

        public virtual void DisplayInfo()
        {
            Console.WriteLine($"ID: {Id}");
            Console.WriteLine($"Species: {Species}");
            Console.WriteLine($"Age: {(string.IsNullOrEmpty(Age) ? "?" : Age)}");
            Console.WriteLine($"Physical Description: {PhysicalDescription}");
            Console.WriteLine($"Personality Description: {PersonalityDescription}");
            Console.WriteLine($"Nickname: {Nickname}");
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