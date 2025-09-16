using ContosoPets.Application.Ports;
using ContosoPets.Domain.Entities;
using ContosoPets.Infrastructure.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace ContosoPets.UnitTests.Application.Services
{
    public class AnimalServiceSearchTests
    {
        private readonly Mock<IAnimalRepository> _mockRepository;
        private readonly AnimalService _animalService;

        public AnimalServiceSearchTests()
        {
            _mockRepository = new Mock<IAnimalRepository>();
            _animalService = new AnimalService(_mockRepository.Object);
        }

        [Fact]
        public void GetAnimalsWithIncompleteAgeOrDescription_ShouldReturnOnlyIncompleteAnimal()
        {
            // Arrange
            var animals = new List<Animal>
            {
                new Dog("dog", "d1", "?", "Golden fur", "Frieendly", "Rex"), // Incomplete age
                new Cat("cat", "c1", "3 years", "tbd", "Independent", "Whiskers"), // Incomplete physical description
                new Dog("dog", "d2", "2 years", "Black fur", "Playful", "Buddy"), // Complete
                new Cat("cat", "c2", "", "", "Curious", "Mittens") // Empty age and physical description
            };

            var filteredAnimals = animals.Where(a =>
                string.IsNullOrWhiteSpace(a.Age) || a.Age == "?" ||
                string.IsNullOrWhiteSpace(a.PhysicalDescription) || a.PhysicalDescription.ToLower() == "tbd"
            ).ToList();

            _mockRepository.Setup(r => r.GetAnimalsWithIncompleteAgeOrDescription())
                .Returns(filteredAnimals);

            // Act
            var result = _animalService.GetAnimalsWithIncompleteAgeOrDescription();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(a => a.Id == "d1");
            result.Should().Contain(a => a.Id == "c1");
            result.Should().Contain(a => a.Id == "c2");
            result.Should().NotContain(a => a.Id == "d2");
        }

        [Fact]
        public void GetAnimalsWithIncompleteNicknameOrPersonality_ShouldReturnOnlyIncompleteAnimal()
        {
            // Arrange
            var animals = new List<Animal>
            {
                new Dog("dog", "d1", "2 years", "Golden fur", "tbd", "Rex"), // Incomplete personality
                new Cat("cat", "c1", "3 years", "Short hair", "", "Whiskers"), // Empty personality
                new Dog("dog", "d2", "2 years", "Black fur", "Playful", "Buddy"), // Complete
                new Cat("cat", "c2", "1 year", "Tabby", "Curious", "tbd") // Incomplete nickname
            };

            var filteredAnimals = animals.Where(a =>
                string.IsNullOrWhiteSpace(a.PersonalityDescription) || a.PersonalityDescription.ToLower() == "tbd" ||
                string.IsNullOrWhiteSpace(a.Nickname) || a.Nickname.ToLower() == "tbd"
            ).ToList();

            _mockRepository.Setup(r => r.GetAnimalsWithIncompleteNicknameOrPersonality())
                .Returns(filteredAnimals);

            // Act
            var result = _animalService.GetAnimalsWithIncompleteNicknameOrPersonality();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(a => a.Id == "d1");
            result.Should().Contain(a => a.Id == "c1");
            result.Should().Contain(a => a.Id == "c2");
            result.Should().NotContain(a => a.Id == "d2");

            
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
            else if (species.Equals("cat", StringComparison.OrdinalIgnoreCase))
            {
                if (characteristic.Equals("black", StringComparison.OrdinalIgnoreCase))
                {
                    filteredResults.Add(new Cat("cat", "c1", "1 year", "Tabby with black stripes", "Curious and independent", "Whiskers"));
                }
            }
            _mockRepository.Setup(r => r.GetAnimalsWithCharacteristic(species, characteristic))
                .Returns(filteredResults);

            // Act
            var result = _animalService.GetAnimalsWithCharacteristic(species, characteristic);

            // Assert
            result.Should().HaveCount(expectedCount);          
        }
    }
}
