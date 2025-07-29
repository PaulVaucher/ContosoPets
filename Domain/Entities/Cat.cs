namespace ContosoPets.Domain.Entities
{
    public class Cat : Animal
    {
        protected Cat() : base() { }
        public Cat(string id, string age, string physicalDescription, string personalityDescription, string nickname)
            : base("cat", id, age, physicalDescription, personalityDescription, nickname)
        {
        }
    }
}