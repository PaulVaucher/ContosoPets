namespace ContosoPets.Domain.Entities
{
    public class Cat : Animal
    {
        public Cat() : base() { }

        public Cat(string species, string id, string age, string physicalDescription, string personalityDescription, string nickname)
            : base(species, id, age, physicalDescription, personalityDescription, nickname)
        {
        }
    }
}