using ContosoPets.Application.Ports;
using ContosoPets.Application.Services;
using ContosoPets.Domain.Constants;
using ContosoPets.Presentation.ConsoleApp.UI;
using FluentAssertions;
using Moq;
using Xunit;

namespace ContosoPets.UnitTests.Presentation.UI
{
    public class MenuHandlerTests
    {
        private readonly Mock<IAnimalApplicationService> _mockService;
        private readonly Mock<ILinePrinter> _mockOutput;
        private readonly MenuHandler _menuHandler;

        public MenuHandlerTests()
        {
            _mockService = new Mock<IAnimalApplicationService>();
            _mockOutput = new Mock<ILinePrinter>();
            _menuHandler = new MenuHandler(_mockService.Object, _mockOutput.Object);
        }

        [Fact]
        public void Constructor_WithNullService_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var action = () => new MenuHandler(null!, _mockOutput.Object);
            action.Should().Throw<ArgumentNullException>().WithParameterName("service");
        }

        [Fact]
        public void Constructor_WithNullOutput_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var action = () => new MenuHandler(_mockService.Object, null!);
            action.Should().Throw<ArgumentNullException>().WithParameterName("output");
        }

        [Fact]
        public void RunInteractiveMenu_ShouldDisplayWelcomeMessage()
        {
            // Arrange
            _mockOutput.SetupSequence(x => x.ReadLine())
                      .Returns("0"); // Exit immediately

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
                      .Returns("0"); // Exit command

            // Act
            _menuHandler.RunInteractiveMenu();

            // Assert
            _mockOutput.Verify(x => x.PrintLine(AppConstants.GoodbyeMessage), Times.Once);
        }

        [Fact]
        public void RunInteractiveMenu_WithInvalidInput_ShouldShowErrorMessage()
        {
            // Arrange
            _mockOutput.SetupSequence(x => x.ReadLine())
                      .Returns("invalid")
                      .Returns("0"); // Exit after invalid input

            // Act
            _menuHandler.RunInteractiveMenu();

            // Assert
            _mockOutput.Verify(x => x.PrintLine(AppConstants.InvalidOptionMessage), Times.Once);
        }
    }
}