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
            var animal = new Dog("dog", "d1", "old", "Golden fur", "Friendly", "Rex");

            _mockRepository.Setup(r => r.GetById("d1")).Returns(animal);

            // Act
            var result = _animalService.UpdateAnimalAge("d1", "5 years");

            // Assert
            result.Should().BeTrue();
            animal.Age.Should().Be("5 years");
            _mockRepository.Verify(r => r.UpdateAnimal(animal), Times.Once);
        }

        [Fact]
        public void UpdateAnimalAge_WithNonExistentId_ShouldReturnFalse()
        {
            // Arrange            
            _mockRepository.Setup(r => r.GetById("nonexistent")).Returns((Animal?)null);

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
            var dog = new Dog("dog", "d1", "?", "tbd", "Friendly", "Rex");
            var cat = new Cat("cat", "c1", "old", "black", "Independent", "Whiskers");

            _mockRepository.Setup(r => r.GetById("d1")).Returns(dog);
            _mockRepository.Setup(r => r.GetById("c1")).Returns(cat);

            var corrections = new Dictionary<string, (string Age, string PhysicalDescription)>
            {
                ["d1"] = ("3 years", "Golden fur"),
                ["c1"] = ("2 years", "Silky black")
            };

            // Act
            _animalService.CompleteAgesAndDescriptions(corrections);

            // Assert
            dog.Age.Should().Be("3 years");
            dog.PhysicalDescription.Should().Be("Golden fur");
            cat.Age.Should().Be("2 years");
            cat.PhysicalDescription.Should().Be("Silky black");
            _mockRepository.Verify(r => r.UpdateAnimal(dog), Times.Once);
            _mockRepository.Verify(r => r.UpdateAnimal(cat), Times.Once);            
        }
    }
}
