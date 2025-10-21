using ContosoPets.Application.Ports;
using ContosoPets.Application.Services;
using ContosoPets.Domain.Entities;
using ContosoPets.Domain.Services;
using ContosoPets.UnitTests.TestInfrastructure.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ContosoPets.UnitTests.Application.Services
{
    public class AnimalApplicationServiceSearchTests
    {
        private readonly Mock<ILogger<AnimalApplicationService>> _mockLogger;
        private readonly FakeAnimalRepository _fakeRepository;
        private readonly FakeAnimalDomainService _fakeDomainService;
        private readonly AnimalApplicationService _animalService;

        public AnimalApplicationServiceSearchTests()
        {
            _mockLogger = new Mock<ILogger<AnimalApplicationService>>();
            _fakeRepository = new FakeAnimalRepository();
            _fakeDomainService = new FakeAnimalDomainService();
            _animalService = new AnimalApplicationService(_mockLogger.Object, _fakeRepository, _fakeDomainService);
        }        

        [Fact]
        public void GetAnimalsWithIncompleteAgeOrDescription_ShouldReturnFilteredResults()
        {
            //Arrange
            _fakeRepository.SeedWith(
                new Dog("dog", "d1", "?", "Golden fur", "Friendly", "Rex"), // Incomplete age
                new Cat("cat", "c2", "3 years", "tbd", "Independent", "Whiskers"), // Incomplete description
                new Dog("dog", "d3", "2 years", "Black fur", "Playful", "Buddy"), // Complete
                new Cat("cat", "c4", "", "", "Curious", "Mittens") // Empty values
            );

            //Act
            var result = _animalService.GetAnimalsWithIncompleteAgeOrDescription();

            //Assert
            result.Should().HaveCount(3);
            result.Should().Contain(a => a.Id.Value == "d1");
            result.Should().Contain(a => a.Id.Value == "c2");
            result.Should().Contain(a => a.Id.Value == "c4");
            result.Should().NotContain(a => a.Id.Value == "d3");
        }

        [Fact]
        public void GetAnimalsWithIncompleteNicknameOrPersonality_ShouldReturnFilteredResults()
        {
            // Arrange            
            _fakeRepository.SeedWith(
                new Dog("dog", "d1", "2 years", "Golden fur", "tbd", "Rex"), // Incomplete personality
                new Cat("cat", "c2", "3 years", "Short hair", "", "Whiskers"), // Empty personality
                new Dog("dog", "d3", "2 years", "Black fur", "Playful", "Buddy"), // Complete
                new Cat("cat", "c4", "1 year", "Tabby", "Curious", "tbd") // Incomplete nickname
            );

            // Act
            var result = _animalService.GetAnimalsWithIncompleteNicknameOrPersonality();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(a => a.Id.Value == "d1");
            result.Should().Contain(a => a.Id.Value == "c2");
            result.Should().Contain(a => a.Id.Value == "c4");
            result.Should().NotContain(a => a.Id.Value == "d3");
        }

        [Fact]
        public void GetAnimalsWithCharacteristic_ShouldFilterCorrectly()
        {
            // Arrange
            _fakeRepository.SeedWith(
                new Dog("dog", "d1", "2 years", "Golden fur", "Friendly and energetic", "Rex"),
                new Dog("dog", "d3", "3 years", "Black fur", "Calm", "Shadow"),
                new Cat("cat", "c2", "1 year", "White fur", "Playful", "Snow")
            );

            // Act
            var friendlyDogs = _animalService.GetAnimalsWithCharacteristic("dog", "friendly");
            var goldenAnimals = _animalService.GetAnimalsWithCharacteristic("dog", "golden");
            var playfulCats = _animalService.GetAnimalsWithCharacteristic("cat", "playful");

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