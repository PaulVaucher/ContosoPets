using ContosoPets.Domain.Builders;
using ContosoPets.Domain.Entities;
using ContosoPets.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace ContosoPets.UnitTests.Domain.Builders
{
    public class AnimalBuilderTests
    {
        [Fact]
        public void Build_WithValidDogData_ShouldCreateDog()
        {
            // Arrange
            var builder = AnimalBuilder.Builder();

            // Act
            var result = builder
                .WithSpecies("dog")
                .WithId("d1")
                .WithAge("2 years")
                .WithPhysicalDescription("Golden fur")
                .WithPersonalityDescription("Friendly")
                .WithNickname("Rex")
                .Build();

            // Assert
            result.Should().BeOfType<Dog>();
            result.Species.Should().Be("dog");
            result.Id.Value.Should().Be("d1");
            result.Age.Should().Be("2 years");
            result.Nickname.Should().Be("Rex");

        }

        [Fact]
        public void Build_WithoutSpecies_ShouldThrowException()
        {
            // Arrange
            var builder = AnimalBuilder.Builder();

            // Act
            Action act = () => builder
                .WithId("d1")
                .Build();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Invalid input. Please enter 'dog' or 'cat'.");
        }

        [Fact]
        public void Build_WithoutId_ShouldThrowException()
        {
            // Arrange
            var builder = AnimalBuilder.Builder();

            // Act
            Action act = () => builder
                .WithSpecies("dog")
                .Build();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("ID must be specified.");
        }

        [Fact]
        public void Build_WithMultipleBuilds_ShouldResetBetweenBuilds()
        {
            // Arrange
            var builder = AnimalBuilder.Builder();

            // Act - First Build
            var firstAnimal = builder
                .WithSpecies("dog")
                .WithId("d1")
                .WithNickname("Rex")
                .Build();

            // Act - Second Build
            var secondAnimal = builder
                .WithSpecies("cat")
                .WithId("c1")
                .WithNickname("Whiskers")
                .Build();

            // Assert
            firstAnimal.Should().BeOfType<Dog>();
            secondAnimal.Should().BeOfType<Cat>();
            secondAnimal.Nickname.Should().Be("Whiskers");
        }

        [Theory]
        [InlineData("dog", typeof(Dog))]
        [InlineData("cat", typeof(Cat))]
        [InlineData("DOG", typeof(Dog))]
        [InlineData("Cat", typeof(Cat))]
        public void Build_WithDifferentSpecies_ShouldCreateCorrectType(string species, Type expectedType)
        {
            // Arrange & Act
            var result = AnimalBuilder.Builder()
                .WithSpecies(species)
                .WithId("test1")
                .Build();

            // Assert
            result.Should().BeOfType(expectedType);
            result.Species.Should().Be(species.ToLower());
        }

        [Theory]
        [InlineData("")]
        [InlineData("bird")]
        [InlineData("fish")]
        [InlineData("invalid")]
        public void Build_WithInvalidSpecies_ShouldThrowException(string invalidSpecies)
        {
            // Arrange & Act
            Action act = () => AnimalBuilder.Builder()
                .WithSpecies(invalidSpecies)
                .WithId("test1")
                .Build();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Invalid input. Please enter 'dog' or 'cat'.");
        }
    }
}
