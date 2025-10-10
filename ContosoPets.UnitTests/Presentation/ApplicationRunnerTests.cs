using ContosoPets.Application.Ports;
using ContosoPets.Application.Services;
using ContosoPets.Domain.Constants;
using ContosoPets.Presentation.ConsoleApp;
using ContosoPets.Presentation.ConsoleApp.UI;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ContosoPets.UnitTests.Presentation
{
    public class ApplicationRunnerTests
    {
        [Fact]
        public void RunApplication_WithNullServiceProvider_ShouldThrowArgumentNullException()
        {
            // Arrange 
            var mockLogger = new Mock<ILogger<object>>();

            // Act & Assert
            var action = () => ApplicationRunner.RunApplication(null!, mockLogger.Object);
            action.Should().Throw<ArgumentNullException>().WithParameterName("provider");
        }

        [Fact]
        public void RunApplication_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange
            var mockMenuLogger = new Mock<ILogger<MenuHandler>>();
            var mockAnimalService = new Mock<IAnimalApplicationService>();
            var mockOutput = new Mock<ILinePrinter>();

            var services = new ServiceCollection();
            services.AddSingleton(mockMenuLogger.Object);
            services.AddSingleton(mockAnimalService.Object);
            services.AddSingleton(mockOutput.Object);

            using var serviceProvider = services.BuildServiceProvider();

            // Act & Assert
            var action = () => ApplicationRunner.RunApplication(serviceProvider, null!);
            action.Should().Throw<ArgumentNullException>().WithParameterName("logger");
        }

        [Fact]
        public void RunApplication_WithValidParameters_ShouldLogApplicationStarted()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<object>>();
            var mockMenuLogger = new Mock<ILogger<MenuHandler>>();
            var mockAnimalService = new Mock<IAnimalApplicationService>();
            var mockOutput = new Mock<ILinePrinter>();

            mockOutput.Setup(x => x.ReadLine()).Returns("0"); // Simulate exit command

            var services = new ServiceCollection();
            services.AddSingleton(mockMenuLogger.Object);
            services.AddSingleton(mockAnimalService.Object);
            services.AddSingleton(mockOutput.Object);

            using var serviceProvider = services.BuildServiceProvider();

            // Act
            ApplicationRunner.RunApplication(serviceProvider, mockLogger.Object);

            // Assert
            mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? string.Empty).Contains(LoggingConstants.ApplicationStarted)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
