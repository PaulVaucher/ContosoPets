using ContosoPets.Application.Ports;
using ContosoPets.Application.Services;
using ContosoPets.Domain.Entities;
using ContosoPets.Domain.Services;
using ContosoPets.UnitTests.TestInfrastructure.Fakes;
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
        public void GetAnimalsWithIncompleteAgeOrDescription_ShouldReturnOnlyIncompleteAnimal()
        {
            // Arrange
            var animals = new List<Animal>
            {
                new Dog("dog", "d1", "?", "Golden fur", "Frieendly", "Rex"), // Incomplete age
                new Cat("cat", "c2", "3 years", "tbd", "Independent", "Whiskers"), // Incomplete physical description
                new Dog("dog", "d3", "2 years", "Black fur", "Playful", "Buddy"), // Complete
                new Cat("cat", "c4", "", "", "Curious", "Mittens") // Empty age and physical description
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
            result.Should().Contain(a => a.Id.Value == "d1");
            result.Should().Contain(a => a.Id.Value == "c2");
            result.Should().Contain(a => a.Id.Value == "c4");
            result.Should().NotContain(a => a.Id == "d3");
        }

        [Fact]
        public void GetAnimalsWithIncompleteNicknameOrPersonality_ShouldReturnOnlyIncompleteAnimal()
        {
            // Arrange
            var animals = new List<Animal>
            {
                new Dog("dog", "d1", "2 years", "Golden fur", "tbd", "Rex"), // Incomplete personality
                new Cat("cat", "c2", "3 years", "Short hair", "", "Whiskers"), // Empty personality
                new Dog("dog", "d3", "2 years", "Black fur", "Playful", "Buddy"), // Complete
                new Cat("cat", "c4", "1 year", "Tabby", "Curious", "tbd") // Incomplete nickname
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
            result.Should().Contain(a => a.Id.Value == "d1");
            result.Should().Contain(a => a.Id.Value == "c2");
            result.Should().Contain(a => a.Id.Value == "c4");
            result.Should().NotContain(a => a.Id.Value == "d3");


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
                filteredResults.Add(new Cat("cat", "c2", "1 year", "Tabby with black stripes", "Curious and independent", "Whiskers"));                
            }
            _mockRepository.Setup(r => r.GetAnimalsWithCharacteristic(species, characteristic))
                .Returns(filteredResults);

            // Act
            var result = _animalService.GetAnimalsWithCharacteristic(species, characteristic);

            // Assert
            result.Should().HaveCount(expectedCount);
        }

        [Fact]
        public void GetAnimalsWithIncompleteAgeOrDescription_ShouldReturnFilteredResults_WithFakes()
        {
            //Arrange
            // Arrange
            var fakeRepository = new FakeAnimalRepository();
            var fakeDomainService = new FakeAnimalDomainService();
            var service = new AnimalApplicationService(fakeRepository, fakeDomainService);

            fakeRepository.SeedWith(
                new Dog("dog", "d1", "?", "Golden fur", "Friendly", "Rex"), // Incomplete age
                new Cat("cat", "c2", "3 years", "tbd", "Independent", "Whiskers"), // Incomplete description
                new Dog("dog", "d3", "2 years", "Black fur", "Playful", "Buddy"), // Complete
                new Cat("cat", "c4", "", "", "Curious", "Mittens") // Empty values
            );

            //Act
            var result = service.GetAnimalsWithIncompleteAgeOrDescription();

            //Assert
            result.Should().HaveCount(3);
            result.Should().Contain(a => a.Id.Value == "d1");
            result.Should().Contain(a => a.Id.Value == "c2");
            result.Should().Contain(a => a.Id.Value == "c4");
            result.Should().NotContain(a => a.Id.Value == "d3");
        }

        [Fact]
        public void GetAnimalsWithIncompleteNicknameOrPersonality_ShouldReturnFilteredResults_WithFakes()
        {
            // Arrange
            var fakeRepository = new FakeAnimalRepository();
            var fakeDomainService = new FakeAnimalDomainService();
            var service = new AnimalApplicationService(fakeRepository, fakeDomainService);

            fakeRepository.SeedWith(
                new Dog("dog", "d1", "2 years", "Golden fur", "tbd", "Rex"), // Incomplete personality
                new Cat("cat", "c2", "3 years", "Short hair", "", "Whiskers"), // Empty personality
                new Dog("dog", "d3", "2 years", "Black fur", "Playful", "Buddy"), // Complete
                new Cat("cat", "c4", "1 year", "Tabby", "Curious", "tbd") // Incomplete nickname
            );

            // Act
            var result = service.GetAnimalsWithIncompleteNicknameOrPersonality();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(a => a.Id.Value == "d1");
            result.Should().Contain(a => a.Id.Value == "c2");
            result.Should().Contain(a => a.Id.Value == "c4");
            result.Should().NotContain(a => a.Id.Value == "d3");
        }

        [Fact]
        public void GetAnimalsWithCharacteristic_ShouldFilterCorrectly_WithFakes()
        {
            // Arrange
            var fakeRepository = new FakeAnimalRepository();
            var fakeDomainService = new FakeAnimalDomainService();
            var service = new AnimalApplicationService(fakeRepository, fakeDomainService);

            fakeRepository.SeedWith(
                new Dog("dog", "d1", "2 years", "Golden fur", "Friendly and energetic", "Rex"),
                new Dog("dog", "d3", "3 years", "Black fur", "Calm", "Shadow"),
                new Cat("cat", "c2", "1 year", "White fur", "Playful", "Snow")
            );

            // Act
            var friendlyDogs = service.GetAnimalsWithCharacteristic("dog", "friendly");
            var goldenAnimals = service.GetAnimalsWithCharacteristic("dog", "golden");
            var playfulCats = service.GetAnimalsWithCharacteristic("cat", "playful");

            // Assert
            friendlyDogs.Should().HaveCount(1);
            friendlyDogs[0].Id.Value.Should().Be("d1");

            goldenAnimals.Should().HaveCount(1);
            goldenAnimals[0].Id.Value.Should().Be("d1");

            playfulCats.Should().HaveCount(1);
            playfulCats[0].Id.Value.Should().Be("c2");
        }
    }
}