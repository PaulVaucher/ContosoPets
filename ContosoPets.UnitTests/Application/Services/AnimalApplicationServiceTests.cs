using ContosoPets.Application.Ports;
using ContosoPets.Application.Services;
using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Domain.Constants;
using ContosoPets.Domain.Entities;
using ContosoPets.Domain.Services;
using ContosoPets.UnitTests.TestInfrastructure.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ContosoPets.UnitTests.Application.Services
{
    public class AnimalApplicationServiceTests
    {
        private readonly Mock<ILogger<AnimalApplicationService>> _mockLogger;
        private readonly Mock<IAnimalRepository> _mockRepository;
        private readonly Mock<IAnimalDomainService> _mockDomainService;
        private readonly AnimalApplicationService _animalService;

        public AnimalApplicationServiceTests()
        {
            _mockLogger = new Mock<ILogger<AnimalApplicationService>>();
            _mockRepository = new Mock<IAnimalRepository>();
            _mockDomainService = new Mock<IAnimalDomainService>();
            _animalService = new AnimalApplicationService(_mockLogger.Object, _mockRepository.Object, _mockDomainService.Object);
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
            result.Animal.Id.Value.Should().Be("d1");

            _mockRepository.Verify(r => r.AddAnimal(expectedAnimal), Times.Once);
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
        }

        [Fact]
        public void ListAll_WhenRepositoryReturnsNull_ShouldReturnEmptyList()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllAnimals()).Returns(new List<Animal>());

            // Act
            var result = _animalService.ListAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _mockRepository.Verify(r => r.GetAllAnimals(), Times.Once);
        }

        [Fact]
        public void AddNewAnimal_WhenPetLimitReached_ShouldReturnFailure_WithFakes()
        {
            //Arrange
            var fakeLogger = new Mock<ILogger<AnimalApplicationService>>();
            var fakeRepository = new FakeAnimalRepository();
            var fakeDomainService = new FakeAnimalDomainService();
            var service = new AnimalApplicationService(fakeLogger.Object, fakeRepository, fakeDomainService);

            for (int i = 1; i <= AppConstants.MaxPets; i++)
            {
                fakeRepository.AddAnimal(new Dog("dog", $"d{i}", "2 years", "Brown", "Friendly", $"dog{i}"));
            }

            var request = new AddAnimalRequest()
            {
                Species = "dog",
                Age = "2 years",
                PhysicalDescription = "Brown fur",
                PersonalityDescription = "Friendly",
                Nickname = "Buddy"
            };

            //Act
            var result = service.AddNewAnimal(request);

            //Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be(AppConstants.PetLimitReachedMessage);
            result.Animal.Should().BeNull();

            fakeRepository.GetAnimalCount().Should().Be(8);
        }

        [Fact]
        public void AddNewAnimal_WithValidRequest_ShouldReturnSuccess_WithFakes()
        {
            //Arrange
            var fakeLogger = new Mock<ILogger<AnimalApplicationService>>();
            var fakeRepository = new FakeAnimalRepository();
            var fakeDomainService = new FakeAnimalDomainService();
            var service = new AnimalApplicationService(fakeLogger.Object, fakeRepository, fakeDomainService);

            var request = new AddAnimalRequest()
            {
                Species = "dog",
                Age = "2 years",
                PhysicalDescription = "Golden fur",
                PersonalityDescription = "Friendly",
                Nickname = "Rex"
            };

            //Act
            var result = service.AddNewAnimal(request);

            //Assert
            result.Success.Should().BeTrue();
            result.Animal.Should().NotBeNull();
            result.Animal!.Species.Should().Be("dog");
            result.Animal.Id.Value.Should().Be("d1");
            result.Animal.Age.Should().Be("2 years");
            result.Animal.Nickname.Should().Be("Rex");

            fakeRepository.GetAnimalCount().Should().Be(1);
            var savedAnimal = fakeRepository.GetById("d1");
            savedAnimal.Should().NotBeNull();
            savedAnimal!.Nickname.Should().Be("Rex");
        }

        [Theory]
        [InlineData("")]
        [InlineData("bird")]
        [InlineData("fish")]
        public void AddNewAnimal_WhitInvalidSpecies_ShouldReturnFailure_WithFakes(string invalidSpecies)
        {
            // Arrange
            var fakeLogger = new Mock<ILogger<AnimalApplicationService>>();
            var fakeRepository = new FakeAnimalRepository();
            var fakeDomainService = new FakeAnimalDomainService();
            var service = new AnimalApplicationService(fakeLogger.Object, fakeRepository, fakeDomainService);

            var request = new AddAnimalRequest
            {
                Species = invalidSpecies,
                Age = "2 years",
                PhysicalDescription = "Description",
                PersonalityDescription = "Personality",
                Nickname = "Name"
            };

            //Act
            var result = service.AddNewAnimal(request);

            //Assert
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be(AppConstants.InvalidSpeciesMessage);
            result.Animal.Should().BeNull();
            fakeRepository.GetAnimalCount().Should().Be(0);
        }

        [Fact]
        public void ListAll_WithMultipleAnimals_ShouldReturnAllAnimals_WithFakes()
        {
            //Arrange
            var fakeLogger = new Mock<ILogger<AnimalApplicationService>>();
            var fakeRepository = new FakeAnimalRepository();
            var fakeDomainService = new FakeAnimalDomainService();
            var service = new AnimalApplicationService(fakeLogger.Object, fakeRepository, fakeDomainService);

            var dog = new Dog("dog", "d1", "2 years", "Golden", "Friendly", "Rex");
            var cat = new Cat("cat", "c2", "3 years", "Black", "Independent", "Whiskers");
            fakeRepository.SeedWith(dog, cat);

            //Act
            var result = service.ListAll();

            //Assert
            result.Should().HaveCount(2);
            result.Should().Contain(a => a.Id.Value == "d1");
            result.Should().Contain(a => a.Id.Value == "c2");
            result.Should().Contain(a => a.Species == "dog");
            result.Should().Contain(a => a.Species == "cat");
        }
    }
}