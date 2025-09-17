using ContosoPets.Application.Ports;
using ContosoPets.Application.Services;
using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Domain.Entities;
using ContosoPets.Domain.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace ContosoPets.UnitTests.Application.Services
{
    public class AnimalApplicationServiceTests
    {
        private readonly Mock<IAnimalRepository> _mockRepository;
        private readonly Mock<IAnimalDomainService> _mockDomainService;
        private readonly AnimalApplicationService _animalService;

        public AnimalApplicationServiceTests()
        {
            _mockRepository = new Mock<IAnimalRepository>();
            _mockDomainService = new Mock<IAnimalDomainService>();
            _animalService = new AnimalApplicationService(_mockRepository.Object, _mockDomainService.Object);
        }

        [Fact]
        public void AddNewAnimal_WhenPetLimitReached_ShouldReturnFailure()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAnimalCount()).Returns(8);
            _mockDomainService.Setup(d => d.ValidateNewAnimal("dog", 8))
                .Returns("We have reached our limit on the number of pets that we can manage.");

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
            var expectedAnimal = new Dog("dog", "d1", "2 years", "Golden fur", "Friendly", "Rex");

            _mockRepository.Setup(r => r.GetAnimalCount()).Returns(0);
            _mockDomainService.Setup(d => d.ValidateNewAnimal("dog", 0)).Returns((string?)null);
            _mockDomainService.Setup(d => d.GenerateId("dog", 1)).Returns("d1");
            _mockDomainService.Setup(d => d.BuildAnimal("dog", "d1", "2 years", "Golden fur", "Friendly", "Rex"))
                .Returns(expectedAnimal);

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

            _mockRepository.Verify(r => r.AddAnimal(expectedAnimal), Times.Once);
            _mockRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("bird")]
        [InlineData("invalid")]
        public void AddNewAnimal_WithInvalidSpecies_ShouldReturnFailure(string invalidSpecies)
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAnimalCount()).Returns(0);
            _mockDomainService.Setup(d => d.ValidateNewAnimal(invalidSpecies, 0))
                .Returns("Invalid input. Please enter 'dog' or 'cat'.");

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