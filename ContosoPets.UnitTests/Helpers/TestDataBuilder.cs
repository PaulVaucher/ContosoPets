using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Domain.Entities;

namespace ContosoPets.UnitTests.Helpers
{
    public static class TestDataBuilder
    {
        public static Dog CreateValidDog(string? id = null, string? age = null)
        {
            return new Dog(
                "dog",
                id ?? "d1",
                age ?? "2 years",
                "Golden fur",
                "Friendly",
                "Rex"
            );
        }

        public static Cat CreateValidCat(string? id = null)
        {
            return new Cat(
                "cat",
                id ?? "c1",
                "3 years",
                "Black fur",
                "Independent",
                "Whiskers"
            );
        }

        public static AddAnimalRequest CreateValidAddAnimalRequest(string? species = null)
        {
            return new AddAnimalRequest
            {
                Species = species ?? "dog",
                Age = "2 years",
                PhysicalDescription = "Golden fur",
                PersonalityDescription = "Friendly",
                Nickname = "Rex"
            };
        }
    }
}
