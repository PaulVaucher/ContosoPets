namespace ContosoPets.Domain.Entities
{
    public class Dog : Animal
    {
        protected Dog() : base() { }
        public Dog(string id, string age, string physicalDescription, string personalityDescription, string nickname)
            : base("dog", id, age, physicalDescription, personalityDescription, nickname)
        {
        }
    }
}