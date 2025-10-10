using ContosoPets.Application.Ports;
using ContosoPets.Application.Services;
using ContosoPets.Domain.Constants;
using ContosoPets.Presentation.ConsoleApp.UI;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ContosoPets.UnitTests.Presentation.UI
{
    public class MenuHandlerTests
    {
        private readonly Mock<ILogger<MenuHandler>> _mockLogger;
        private readonly Mock<IAnimalApplicationService> _mockAnimalService;
        private readonly Mock<ILinePrinter> _mockOutput;
        private readonly MenuHandler _menuHandler;

        public MenuHandlerTests()
        {
            _mockLogger = new Mock<ILogger<MenuHandler>>();
            _mockAnimalService = new Mock<IAnimalApplicationService>();
            _mockOutput = new Mock<ILinePrinter>();
            _menuHandler = new MenuHandler(_mockLogger.Object, _mockAnimalService.Object, _mockOutput.Object);
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Act
            var action = () => new MenuHandler(null!, _mockAnimalService.Object, _mockOutput.Object);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("logger");
        }

        [Fact]
        public void Constructor_WithNullService_ShouldThrowArgumentNullException()
        {
            // Act
            var action = () => new MenuHandler(_mockLogger.Object, null!, _mockOutput.Object);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("service");
        }

        [Fact]
        public void Constructor_WithNullOutput_ShouldThrowArgumentNullException()
        {
            // Act
            var action = () => new MenuHandler(_mockLogger.Object, _mockAnimalService.Object, null!);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithParameterName("output");
        }

        [Fact]
        public void Constructor_WithValidParameters_ShouldNotThrow()
        {
            // Act
            var action = () => new MenuHandler(_mockLogger.Object, _mockAnimalService.Object, _mockOutput.Object);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void RunInteractiveMenu_ShouldDisplayWelcomeMessage()
        {
            // Arrange
            _mockOutput.SetupSequence(x => x.ReadLine())
                .Returns("0");

            // Act
            _menuHandler.RunInteractiveMenu();

            // Assert
            _mockOutput.Verify(x => x.PrintLine(AppConstants.WelcomeMessage), Times.Once);
        }

        [Fact]
        public void RunInteractiveMenu_WithExitCommand_ShouldExitGracefully()
        {
            // Arrange
            _mockOutput.SetupSequence(x => x.ReadLine())
                .Returns("0");

            // Act
            _menuHandler.RunInteractiveMenu();

            // Assert
            _mockOutput.Verify(x => x.PrintLine(AppConstants.GoodbyeMessage), Times.Once);
        }

        [Fact]
        public void RunInteractiveMenu_WithInvalidInput_ShouldLogErrorAndPromptAgain()
        {
            // Arrange
            _mockOutput.SetupSequence(x => x.ReadLine())
                .Returns("invalid")
                .Returns("0");

            // Act
            _menuHandler.RunInteractiveMenu();

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? string.Empty).Contains(
                        "Invalid user input received for MenuSelection: invalid")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
            _mockOutput.Verify(x => x.PrintLine(AppConstants.InvalidOptionMessage), Times.Once);
        }
    }
}
