using ContosoPets.Application.Ports;
using ContosoPets.Application.Services;
using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Domain.Constants;
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
            _mockDomainService.Setup(s => s.ValidateNewAnimal("dog", 8))
                .Returns(AppConstants.PetLimitReachedMessage);

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
            result.ErrorMessage.Should().Be(AppConstants.PetLimitReachedMessage);
            result.Animal.Should().BeNull();

            _mockRepository.Verify(r => r.AddAnimal(It.IsAny<Animal>()), Times.Never);
            _mockRepository.Verify(r => r.SaveChanges(), Times.Never);
        }

        [Fact]
        public void AddNewAnimal_WithValidRequest_ShouldReturnSuccess()
        {
            // Arrange
            var expectedAnimal = new Dog("dog", "d1", "2 years", "Golden fur", "Friendly", "Rex");

            _mockRepository.Setup(r => r.GetAnimalCount()).Returns(0);
            _mockDomainService.Setup(s => s.ValidateNewAnimal("dog", 0)).Returns((string?)null);
            _mockDomainService.Setup(s => s.GenerateId("dog", 1)).Returns("d1");
            _mockDomainService.Setup(s => s.BuildAnimal("dog", "d1", "2 years", "Golden fur", "Friendly", "Rex"))
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
            result.Animal.Id.Should().Be("d1");

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
            _mockDomainService.Setup(s => s.ValidateNewAnimal(invalidSpecies, 0))
                .Returns(AppConstants.InvalidSpeciesMessage);

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
            result.ErrorMessage.Should().Be(AppConstants.InvalidSpeciesMessage);
            result.Animal.Should().BeNull();

            _mockRepository.Verify(r => r.AddAnimal(It.IsAny<Animal>()), Times.Never);
            _mockRepository.Verify(r => r.SaveChanges(), Times.Never);
        }

        [Fact]
        public void ListAll_WhenRepositoryReturnsNull_ShouldReturnEmptyList()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllAnimals()).Returns((List<Animal>?)null);

            // Act
            var result = _animalService.ListAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _mockRepository.Verify(r => r.GetAllAnimals(), Times.Once);
        }
    }
}