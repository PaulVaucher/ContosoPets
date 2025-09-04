namespace ContosoPets.Infrastructure.Entities
{
    public class NHDog : NHAnimal
    {
        public NHDog() { }

        public NHDog(string species)
        {
            Species = species;
        }
    }
}
