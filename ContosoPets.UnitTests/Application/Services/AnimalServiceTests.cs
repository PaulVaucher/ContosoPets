using ContosoPets.Application.Ports;
using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Domain.Entities;
using ContosoPets.Infrastructure.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace ContosoPets.UnitTests.Application.Services
{
    public class AnimalServiceTests
    {
        private readonly Mock<IAnimalRepository> _mockRepository;
        private readonly AnimalService _animalService;

        public AnimalServiceTests()
        {
            _mockRepository = new Mock<IAnimalRepository>();
            _animalService = new AnimalService(_mockRepository.Object);
        }

        [Fact]
        public void AddNewAnimal_WhenPetLimitReached_ShouldReturnFailure()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAnimalCount()).Returns(8);

            var request = new AddAnimalRequest
            {
                Species = "dog",
                Age = "2 years",
                PhysicalDescription = "Brown fur",
                PersonalityDescription = "Friendly",
                Nickname = "Buddy"
            };

            // Act
            var result = _animalService.AddNewAnimal(request);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("We have reached our limit on the number of pets that we can manage.");
            result.Animal.Should().BeNull();
        }

        [Fact]
        public void AddNewAnimal_WithValidRequest_ShouldReturnSuccess()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAnimalCount()).Returns(0);

            var request = new AddAnimalRequest
            {
                Species = "dog",
                Age = "2 years",
                PhysicalDescription = "Golden fur",
                PersonalityDescription = "Friendly",
                Nickname = "Rex"
            };

            // Act
            var result = _animalService.AddNewAnimal(request);

            // Assert
            result.Success.Should().BeTrue();
            result.Animal.Should().NotBeNull();
            result.Animal!.Species.Should().Be("dog");

            _mockRepository.Verify(r => r.AddAnimal(It.IsAny<Animal>()), Times.Once);
            _mockRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("bird")]
        [InlineData("invalid")]
        public void AddNewAnimal_WhitInvalidSpecies_ShouldReturnFailure(string invalidSpecies)
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAnimalCount()).Returns(0);

            var request = new AddAnimalRequest
            {
                Species = invalidSpecies,
                Age = "2 years",
                PhysicalDescription = "Description",
                PersonalityDescription = "Personality",
                Nickname = "Name"
            };

            // Act
            var result = _animalService.AddNewAnimal(request);

            // Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Invalid input. Please enter 'dog' or 'cat'.");
            result.Animal.Should().BeNull();
        }
    }
}
