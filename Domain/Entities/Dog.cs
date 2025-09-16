namespace ContosoPets.Domain.Entities
{
    public class Dog : Animal
    {
        public Dog() : base() { }

        [System.Text.Json.Serialization.JsonConstructor]
        public Dog(string species, string id, string age, string physicalDescription, string personalityDescription, string nickname)
            : base(species, id, age, physicalDescription, personalityDescription, nickname)
        {
        }
    }
}