using ContosoPets.Application.Ports;
using ContosoPets.Application.Services;
using ContosoPets.Domain.Entities;
using ContosoPets.Domain.Services;
using ContosoPets.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace ContosoPets.UnitTests.Application.Services
{
    public class AnimalApplicationServiceUpdateTests
    {
        private readonly Mock<IAnimalRepository> _mockRepository;
        private readonly Mock<IAnimalDomainService> _mockDomainService;
        private readonly AnimalApplicationService _animalService;

        public AnimalApplicationServiceUpdateTests()
        {
            _mockRepository = new Mock<IAnimalRepository>();
            _mockDomainService = new Mock<IAnimalDomainService>();
            _animalService = new AnimalApplicationService(_mockRepository.Object, _mockDomainService.Object);
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
            _mockRepository.Verify(r => r.SaveChanges(), Times.Once);
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
            _mockRepository.Verify(r => r.SaveChanges(), Times.Never);
        }

        [Fact]
        public void CompleteAgesAndDescriptions_ShouldUpdateOnlySpecifiedFields()
        {
            // Arrange
            var dog = new Dog("dog", "d1", "?", "tbd", "Friendly", "Rex");
            var cat = new Cat("cat", "c1", "old", "black", "Independent", "Whiskers");

            _mockRepository.Setup(r => r.GetById("d1")).Returns(dog);
            _mockRepository.Setup(r => r.GetById("c1")).Returns(cat);

            var corrections = new Dictionary<AnimalId, (string Age, string PhysicalDescription)>
            {
                [new AnimalId("d1")] = ("3 years", "Golden fur"),
                [new AnimalId("c1")] = ("2 years", "Silky black")
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
            _mockRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpdateAnimalPersonality_WithExistingId_ShouldReturnTrueAndUpdate()
        {
            // Arrange
            var animal = new Cat("cat", "c1", "2 years", "Gray fur", "old personality", "Whiskers");

            _mockRepository.Setup(r => r.GetById("c1")).Returns(animal);

            // Act
            var result = _animalService.UpdateAnimalPersonality("c1", "Very playful and curious");

            // Assert
            result.Should().BeTrue();
            animal.PersonalityDescription.Should().Be("Very playful and curious");
            _mockRepository.Verify(r => r.UpdateAnimal(animal), Times.Once);
            _mockRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void CompleteNicknamesAndPersonality_ShouldUpdateOnlySpecifiedFields()
        {
            // Arrange
            var dog = new Dog("dog", "d1", "3 years", "Golden fur", "tbd", "tbd");
            var cat = new Cat("cat", "c1", "2 years", "Black fur", "Independent", "tbd");

            _mockRepository.Setup(r => r.GetById("d1")).Returns(dog);
            _mockRepository.Setup(r => r.GetById("c1")).Returns(cat);

            var corrections = new Dictionary<AnimalId, (string Nickname, string Personality)>
            {
                [new AnimalId("d1")] = ("Buddy", "Friendly and energetic"),
                [new AnimalId("c1")] = ("Shadow", "Mysterious and independent")
            };

            // Act
            _animalService.CompleteNicknamesAndPersonality(corrections);

            // Assert
            dog.Nickname.Should().Be("Buddy");
            dog.PersonalityDescription.Should().Be("Friendly and energetic");
            cat.Nickname.Should().Be("Shadow");
            cat.PersonalityDescription.Should().Be("Mysterious and independent");
            _mockRepository.Verify(r => r.UpdateAnimal(dog), Times.Once);
            _mockRepository.Verify(r => r.UpdateAnimal(cat), Times.Once);
            _mockRepository.Verify(r => r.SaveChanges(), Times.Once);
        }
    }
}