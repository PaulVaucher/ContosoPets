using ContosoPets.Application.Ports;
using ContosoPets.Application.Services;
using ContosoPets.Domain.Entities;
using ContosoPets.Domain.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace ContosoPets.UnitTests.Application.Services
{
    public class AnimalApplicationServiceSearchTests
    {
        private readonly Mock<IAnimalRepository> _mockRepository;
        private readonly Mock<IAnimalDomainService> _mockDomainService;
        private readonly AnimalApplicationService _animalService;

        public AnimalApplicationServiceSearchTests()
        {
            _mockRepository = new Mock<IAnimalRepository>();
            _mockDomainService = new Mock<IAnimalDomainService>();
            _animalService = new AnimalApplicationService(_mockRepository.Object, _mockDomainService.Object);
        }

        [Fact]
        public void ListAll_ShouldReturnAllAnimals()
        {
            // Arrange
            var animals = new List<Animal>
            {
                new Dog("dog", "d1", "2 years", "Golden fur", "Friendly", "Rex"),
                new Cat("cat", "c1", "3 years", "Black fur", "Independent", "Whiskers")
            };

            _mockRepository.Setup(r => r.GetAllAnimals()).Returns(animals);

            // Act
            var result = _animalService.ListAll();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(a => a.Id == "d1");
            result.Should().Contain(a => a.Id == "c1");
        }

        [Fact]
        public void ListAll_WithEmptyRepository_ShouldReturnEmptyList()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllAnimals()).Returns(new List<Animal>());

            // Act
            var result = _animalService.ListAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetAnimalsWithIncompleteAgeOrDescription_ShouldReturnOnlyIncompleteAnimals()
        {
            // Arrange
            var incompleteAnimals = new List<Animal>
            {
                new Dog("dog", "d1", "?", "Golden fur", "Friendly", "Rex"), // Incomplete age
                new Cat("cat", "c1", "3 years", "tbd", "Independent", "Whiskers"), // Incomplete physical description
                new Cat("cat", "c2", "", "", "Curious", "Mittens") // Empty age and physical description
            };

            _mockRepository.Setup(r => r.GetAnimalsWithIncompleteAgeOrDescription())
                .Returns(incompleteAnimals);

            // Act
            var result = _animalService.GetAnimalsWithIncompleteAgeOrDescription();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(a => a.Id == "d1");
            result.Should().Contain(a => a.Id == "c1");
            result.Should().Contain(a => a.Id == "c2");
        }

        [Fact]
        public void GetAnimalsWithIncompleteNicknameOrPersonality_ShouldReturnOnlyIncompleteAnimals()
        {
            // Arrange
            var incompleteAnimals = new List<Animal>
            {
                new Dog("dog", "d1", "2 years", "Golden fur", "tbd", "Rex"), // Incomplete personality
                new Cat("cat", "c1", "3 years", "Short hair", "", "Whiskers"), // Empty personality
                new Cat("cat", "c2", "1 year", "Tabby", "Curious", "tbd") // Incomplete nickname
            };

            _mockRepository.Setup(r => r.GetAnimalsWithIncompleteNicknameOrPersonality())
                .Returns(incompleteAnimals);

            // Act
            var result = _animalService.GetAnimalsWithIncompleteNicknameOrPersonality();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(a => a.Id == "d1");
            result.Should().Contain(a => a.Id == "c1");
            result.Should().Contain(a => a.Id == "c2");
        }

        [Theory]
        [InlineData("dog", "friendly", 1)]      // Find in personality
        [InlineData("dog", "golden", 1)]        // Find in physical description
        [InlineData("cat", "black", 1)]         // Test cat
        [InlineData("DOG", "PLAYFUL", 1)]       // Case insensitive
        [InlineData("dog", "nonexistent", 0)]   // No matches
        public void GetAnimalsWithCharacteristic_ShouldFilterCorrectly(
            string species, string characteristic, int expectedCount)
        {
            // Arrange
            var filteredResults = new List<Animal>();

            if (species.Equals("dog", StringComparison.OrdinalIgnoreCase))
            {
                if (characteristic.Equals("friendly", StringComparison.OrdinalIgnoreCase) ||
                    characteristic.Equals("playful", StringComparison.OrdinalIgnoreCase))
                {
                    filteredResults.Add(new Dog("dog", "d1", "2 years", "Golden fur", "Friendly and playful", "Rex"));
                }
                else if (characteristic.Equals("golden", StringComparison.OrdinalIgnoreCase))
                {
                    filteredResults.Add(new Dog("dog", "d1", "2 years", "Golden fur", "Friendly and playful", "Rex"));
                }
            }
            else if (species.Equals("cat", StringComparison.OrdinalIgnoreCase) && characteristic.Equals("black", StringComparison.OrdinalIgnoreCase))
            {
                filteredResults.Add(new Cat("cat", "c1", "1 year", "Tabby with black stripes", "Curious and independent", "Whiskers"));
            }

            _mockRepository.Setup(r => r.GetAnimalsWithCharacteristic(species, characteristic))
                .Returns(filteredResults);

            // Act
            var result = _animalService.GetAnimalsWithCharacteristic(species, characteristic);

            // Assert
            result.Should().HaveCount(expectedCount);
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
            result.Should().NotBeNull();
            result!.Id.Should().Be("d1");
            result.Species.Should().Be("dog");
        }

        [Fact]
        public void GetAnimalById_WithNonExistentId_ShouldReturnNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetById("nonexistent")).Returns((Animal?)null);

            // Act
            var result = _animalService.GetAnimalById("nonexistent");

            // Assert
            result.Should().BeNull();
        }
    }
}