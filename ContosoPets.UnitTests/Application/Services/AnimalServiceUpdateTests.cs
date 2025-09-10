using ContosoPets.Application.Ports;
using ContosoPets.Domain.Entities;
using ContosoPets.Infrastructure.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace ContosoPets.UnitTests.Application.Services
{
    public class AnimalServiceUpdateTests
    {
        private readonly Mock<IAnimalRepository> _mockRepository;
        private readonly AnimalService _animalService;


        public AnimalServiceUpdateTests()
        {
            _mockRepository = new Mock<IAnimalRepository>();
            _animalService = new AnimalService(_mockRepository.Object);
        }

        [Fact]
        public void UpdateAnimalAge_WithExistingId_ShouldReturnTrueAndUpdate()
        {
            // Arrange
            var animals = new List<Animal>
            {
                new Dog("dog", "d1", "old", "Golden fur", "Friendly", "Rex")
            };
            _mockRepository.Setup(r => r.GetAllAnimals()).Returns(animals);

            // Act
            var result = _animalService.UpdateAnimalAge("d1", "5 years");

            // Assert
            result.Should().BeTrue();
            animals[0].Age.Should().Be("5 years");
            _mockRepository.Verify(r => r.UpdateAnimal(It.IsAny<Animal>()), Times.Once);
        }

        [Fact]
        public void UpdateAnimalAge_WithNonExistentId_ShouldReturnFalse()
        {
            // Arrange            
            _mockRepository.Setup(r => r.GetAllAnimals()).Returns(new List<Animal>());

            // Act
            var result = _animalService.UpdateAnimalAge("nonexistent", "5 years");

            // Assert
            result.Should().BeFalse();
            _mockRepository.Verify(r => r.UpdateAnimal(It.IsAny<Animal>()), Times.Never);
        }

        [Fact]
        public void CompleteAgesAndDescriptions_ShouldUpdateOnlySpecifiedFileds()
        {
            // Arrange
            var animals = new List<Animal>
            {
                new Dog("dog", "d1", "?", "tbd", "Friendly", "Rex"),
                new Cat("cat", "c1", "old", "black", "Independent", "Whiskers"),
            };
            _mockRepository.Setup(r => r.GetAllAnimals()).Returns(animals);

            var corrections = new Dictionary<string, (string Age, string PhysicalDescription)>
            {
                ["d1"] = ("3 years", "Golden fur"),
                ["c1"] = ("2 years", "Silky black")
            };

            // Act
            _animalService.CompleteAgesAndDescriptions(corrections);

            // Assert
            animals[0].Age.Should().Be("3 years");
            animals[0].PhysicalDescription.Should().Be("Golden fur");
            animals[1].Age.Should().Be("2 years");
            animals[1].PhysicalDescription.Should().Be("Silky black");
            _mockRepository.Verify(r => r.UpdateAnimal(It.IsAny<Animal>()), Times.Exactly(2));
        }
    }
}
