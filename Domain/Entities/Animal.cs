namespace ContosoPets.Domain.Entities
{
    public class Animal
    {
        public string Species { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Age { get; set; } = "?";
        public string PhysicalDescription { get; set; } = string.Empty;
        public string PersonalityDescription { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;

        public Animal() {}

        public Animal(string species, string id, string age, string physicalDescription, string personalityDescription, string nickname)
        {
            Species = species;
            Id = id;
            Age = age;
            PhysicalDescription = physicalDescription;
            PersonalityDescription = personalityDescription;
            Nickname = nickname;
        }

        public void DisplayInfo() 
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
