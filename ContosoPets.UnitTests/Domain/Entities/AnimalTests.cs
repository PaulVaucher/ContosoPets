using ContosoPets.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace ContosoPets.UnitTests.Domain.Entities
{
    public class AnimalTests
    {
        [Theory]
        [InlineData("dog", 1, "d1")]
        [InlineData("cat", 5, "c5")]
        [InlineData("unknown", 1, "u1")]
        [InlineData("DOG", 2, "d2")]
        public void GenerateId_WithDifferentSpeciesAndIndex_ShouldGenerateCorrectId(
            string species, int index, string expectedId)
        {
            // Act
            var result = Animal.GenerateId(species, index);

            // Assert
            result.Should().Be(expectedId);
        }

        [Theory]
        [InlineData(null, "?")]
        [InlineData("", "?")]
        [InlineData("5 years", "5 years")]
        public void SetAge_WithVariousValues_ShouldSetCorrectly(string? inputAge, string expectedAge)
        {
            // Arrange
            var dog = new Dog("dog", "d1", "old", "Golden fur", "Friendly", "Rex");

            // Act
            dog.SetAge(inputAge ?? string.Empty);

            // Assert
            dog.Age.Should().Be(expectedAge);
        }

        [Theory]
        [InlineData(null, "tbd")]
        [InlineData("", "tbd")]
        [InlineData("Beautiful golden fur", "Beautiful golden fur")]
        public void SetPhysicalDescription_WithVariousValues_ShouldHandleCorrectly(
        string? inputDesc, string expectedDesc)
        {
            // Arrange
            var cat = new Cat("cat", "c1", "3 years", "old", "Independent", "Whiskers");

            // Act
            cat.SetPhysicalDescription(inputDesc ?? string.Empty);
            // Assert
            cat.PhysicalDescription.Should().Be(expectedDesc);
        }
    }
}
