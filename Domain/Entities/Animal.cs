namespace ContosoPets.Domain.Entities
{
    public abstract class Animal
    {
        public string Species { get; private set; }
        public virtual string Id { get; private set; }
        public virtual string Age { get; private set; }
        public virtual string PhysicalDescription { get; private set; }
        public virtual string PersonalityDescription { get; private set; }
        public virtual string Nickname { get; private set; }

        protected Animal() {}
        
        protected Animal(string species, string id, string age, string physicalDescription, string personalityDescription, string nickname)
        {
            Species = species;
            Id = id;
            Age = age;
            PhysicalDescription = physicalDescription;
            PersonalityDescription = personalityDescription;
            Nickname = nickname;
        }

        public virtual void SetAge(string value) => Age = value ?? "?";

        public virtual void SetPhysicalDescription(string value) =>
            PhysicalDescription = value ?? "tbd";

        public virtual void SetPersonalityDescription(string value) =>
            PersonalityDescription = value ?? "tbd";

        public virtual void SetNickname(string value) =>
            Nickname = value ?? "tbd";

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
