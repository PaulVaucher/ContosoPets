using ContosoPets.Application.Ports;
using ContosoPets.Domain.Constants;
using ContosoPets.Domain.Entities;
using ContosoPets.Application.Services;
using ContosoPets.Domain.Services;
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
            var animal = new Dog("dog", "d1", "?", "Golden fur", "Friendly", "Rex");

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
        public void UpdateAnimalAge_WithEmptyValue_ShouldSetToDefaultValue()
        {
            // Arrange
            var animal = new Dog("dog", "d1", "2 years", "Golden fur", "Friendly", "Rex");
            _mockRepository.Setup(r => r.GetById("d1")).Returns(animal);

            // Act
            var result = _animalService.UpdateAnimalAge("d1", "");

            // Assert
            result.Should().BeTrue();
            animal.Age.Should().Be("?"); // Default value for empty age
            _mockRepository.Verify(r => r.UpdateAnimal(animal), Times.Once);
            _mockRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void CompleteAgesAndDescriptions_ShouldUpdateOnlySpecifiedFields()
        {
            // Arrange
            var dog = new Dog("dog", "d1", "?", "tbd", "Friendly", "Rex");
            var cat = new Cat("cat", "c1", "?", "tbd", "Independent", "Whiskers");

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
            _mockRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void CompleteAgesAndDescriptions_WithOnlyAge_ShouldUpdateOnlyAge()
        {
            // Arrange
            var dog = new Dog("dog", "d1", "?", "Golden fur", "Friendly", "Rex");
            _mockRepository.Setup(r => r.GetById("d1")).Returns(dog);

            var corrections = new Dictionary<string, (string Age, string PhysicalDescription)>
            {
                ["d1"] = ("3 years", "") // Only age provided, empty description
            };

            // Act
            _animalService.CompleteAgesAndDescriptions(corrections);

            // Assert
            dog.Age.Should().Be("3 years");
            dog.PhysicalDescription.Should().Be("Golden fur"); // Should remain unchanged
            _mockRepository.Verify(r => r.UpdateAnimal(dog), Times.Once);
            _mockRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void CompleteAgesAndDescriptions_WithOnlyPhysicalDescription_ShouldUpdateOnlyDescription()
        {
            // Arrange
            var dog = new Dog("dog", "d1", "2 years", "tbd", "Friendly", "Rex");
            _mockRepository.Setup(r => r.GetById("d1")).Returns(dog);

            var corrections = new Dictionary<string, (string Age, string PhysicalDescription)>
            {
                ["d1"] = ("", "Beautiful golden fur") // Only description provided, empty age
            };

            // Act
            _animalService.CompleteAgesAndDescriptions(corrections);

            // Assert
            dog.Age.Should().Be("2 years"); // Should remain unchanged
            dog.PhysicalDescription.Should().Be("Beautiful golden fur");
            _mockRepository.Verify(r => r.UpdateAnimal(dog), Times.Once);
            _mockRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void CompleteAgesAndDescriptions_WithAllEmptyValues_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var dog = new Dog("dog", "d1", "2 years", "Golden fur", "Friendly", "Rex");
            _mockRepository.Setup(r => r.GetById("d1")).Returns(dog);

            var corrections = new Dictionary<string, (string Age, string PhysicalDescription)>
            {
                ["d1"] = ("", "") // Both empty values
            };

            // Act & Assert
            Action act = () => _animalService.CompleteAgesAndDescriptions(corrections);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*No valid modifications provided*");
        }

        [Fact]
        public void UpdateAnimalPersonality_WithExistingId_ShouldReturnTrueAndUpdate()
        {
            // Arrange
            var animal = new Dog("dog", "d1", "2 years", "Golden fur", "tbd", "Rex");
            _mockRepository.Setup(r => r.GetById("d1")).Returns(animal);

            // Act
            var result = _animalService.UpdateAnimalPersonality("d1", "New friendly personality");

            // Assert
            result.Should().BeTrue();
            animal.PersonalityDescription.Should().Be("New friendly personality");
            _mockRepository.Verify(r => r.UpdateAnimal(animal), Times.Once);
            _mockRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpdateAnimalPersonality_WithEmptyValue_ShouldSetToDefaultValue()
        {
            // Arrange
            var animal = new Dog("dog", "d1", "2 years", "Golden fur", "Friendly", "Rex");
            _mockRepository.Setup(r => r.GetById("d1")).Returns(animal);

            // Act
            var result = _animalService.UpdateAnimalPersonality("d1", "");

            // Assert
            result.Should().BeTrue();
            animal.PersonalityDescription.Should().Be("tbd"); // Default value for empty personality
            _mockRepository.Verify(r => r.UpdateAnimal(animal), Times.Once);
            _mockRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void CompleteNicknamesAndPersonality_WithValidCorrections_ShouldUpdateAnimals()
        {
            // Arrange
            var dog = new Dog("dog", "d1", "2 years", "Golden fur", "tbd", "tbd");
            var cat = new Cat("cat", "c1", "3 years", "Black fur", "tbd", "tbd");

            _mockRepository.Setup(r => r.GetById("d1")).Returns(dog);
            _mockRepository.Setup(r => r.GetById("c1")).Returns(cat);

            var corrections = new Dictionary<string, (string Nickname, string Personality)>
            {
                ["d1"] = ("Rex", "Friendly and playful"),
                ["c1"] = ("Whiskers", "Independent and curious")
            };

            // Act
            _animalService.CompleteNicknamesAndPersonality(corrections);

            // Assert
            dog.Nickname.Should().Be("Rex");
            dog.PersonalityDescription.Should().Be("Friendly and playful");
            cat.Nickname.Should().Be("Whiskers");
            cat.PersonalityDescription.Should().Be("Independent and curious");

            _mockRepository.Verify(r => r.UpdateAnimal(dog), Times.Once);
            _mockRepository.Verify(r => r.UpdateAnimal(cat), Times.Once);
            _mockRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void CompleteNicknamesAndPersonality_WithOnlyNickname_ShouldUpdateOnlyNickname()
        {
            // Arrange
            var dog = new Dog("dog", "d1", "2 years", "Golden fur", "Friendly", "tbd");
            _mockRepository.Setup(r => r.GetById("d1")).Returns(dog);

            var corrections = new Dictionary<string, (string Nickname, string Personality)>
            {
                ["d1"] = ("Rex", "") // Only nickname provided, empty personality
            };

            // Act
            _animalService.CompleteNicknamesAndPersonality(corrections);

            // Assert
            dog.Nickname.Should().Be("Rex");
            dog.PersonalityDescription.Should().Be("Friendly"); // Should remain unchanged
            _mockRepository.Verify(r => r.UpdateAnimal(dog), Times.Once);
            _mockRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void CompleteNicknamesAndPersonality_WithOnlyPersonality_ShouldUpdateOnlyPersonality()
        {
            // Arrange
            var dog = new Dog("dog", "d1", "2 years", "Golden fur", "tbd", "Rex");
            _mockRepository.Setup(r => r.GetById("d1")).Returns(dog);

            var corrections = new Dictionary<string, (string Nickname, string Personality)>
            {
                ["d1"] = ("", "Very friendly and energetic") // Only personality provided, empty nickname
            };

            // Act
            _animalService.CompleteNicknamesAndPersonality(corrections);

            // Assert
            dog.Nickname.Should().Be("Rex"); // Should remain unchanged
            dog.PersonalityDescription.Should().Be("Very friendly and energetic");
            _mockRepository.Verify(r => r.UpdateAnimal(dog), Times.Once);
            _mockRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public void CompleteNicknamesAndPersonality_WithAllEmptyValues_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var dog = new Dog("dog", "d1", "2 years", "Golden fur", "Friendly", "Rex");
            _mockRepository.Setup(r => r.GetById("d1")).Returns(dog);

            var corrections = new Dictionary<string, (string Nickname, string Personality)>
            {
                ["d1"] = ("", "") // Both empty values
            };

            // Act & Assert
            Action act = () => _animalService.CompleteNicknamesAndPersonality(corrections);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*No valid modifications provided*");
        }

        [Fact]
        public void CompleteNicknamesAndPersonality_WithEmptyCorrections_ShouldThrowArgumentException()
        {
            // Arrange
            var corrections = new Dictionary<string, (string Nickname, string Personality)>();

            // Act & Assert
            Action act = () => _animalService.CompleteNicknamesAndPersonality(corrections);

            act.Should().Throw<ArgumentException>()
                .WithMessage(AppConstants.NoCorrectionsProvidedMessage);

            _mockRepository.Verify(r => r.UpdateAnimal(It.IsAny<Animal>()), Times.Never);
            _mockRepository.Verify(r => r.SaveChanges(), Times.Never);
        }

        [Fact]
        public void CompleteNicknamesAndPersonality_WithNullCorrections_ShouldThrowArgumentException()
        {
            // Act & Assert
            Action act = () => _animalService.CompleteNicknamesAndPersonality(null!);

            act.Should().Throw<ArgumentException>()
                .WithMessage(AppConstants.NoCorrectionsProvidedMessage);
        }

        [Fact]
        public void GetAnimalById_WithExistingId_ShouldReturnAnimal()
        {
            // Arrange
            var expectedAnimal = new Dog("dog", "d1", "2 years", "Golden fur", "Friendly", "Rex");
            _mockRepository.Setup(r => r.GetById("d1")).Returns(expectedAnimal);

            // Act
            var result = _animalService.GetAnimalById("d1");

            // Assert
            result.Should().Be(expectedAnimal);
            result.Should().NotBeNull();
            result!.Id.Should().Be("d1");
            result.Species.Should().Be("dog");
            _mockRepository.Verify(r => r.GetById("d1"), Times.Once);
        }

        [Fact]
        public void GetAnimalById_WithValidGeneratedId_ShouldReturnAnimal()
        {
            // Arrange - Test avec un ID qui suit le pattern de génération automatique
            var expectedAnimal = new Cat("cat", "c3", "1 year", "Tabby fur", "Curious", "Mittens");
            _mockRepository.Setup(r => r.GetById("c3")).Returns(expectedAnimal);

            // Act
            var result = _animalService.GetAnimalById("c3");

            // Assert
            result.Should().Be(expectedAnimal);
            result!.Species.Should().Be("cat");
            result.Id.Should().Be("c3");
            _mockRepository.Verify(r => r.GetById("c3"), Times.Once);
        }
    }
}