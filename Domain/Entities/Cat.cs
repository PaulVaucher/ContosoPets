namespace ContosoPets.Domain.Entities
{
    public class Cat : Animal
    {
        public Cat() : base() { }

        [System.Text.Json.Serialization.JsonConstructor]
        public Cat(string id, string age, string physicalDescription, string personalityDescription, string nickname)
            : base("cat", id, age, physicalDescription, personalityDescription, nickname)
        {
        }
    }
}