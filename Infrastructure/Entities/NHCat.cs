namespace ContosoPets.Infrastructure.Entities
{
    public class NHCat : NHAnimal
    {
        public NHCat() { }

        public NHCat(string species)
        {
            Species = species;
        }
    }
}
