using ContosoPets.Application.Ports;
using ContosoPets.Application.Services;
using ContosoPets.Presentation.ConsoleApp;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace ContosoPets.UnitTests.Presentation
{
    public class ApplicationRunnerTests
    {
        [Fact]
        public void RunApplication_WithNullServiceProvider_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var action = () => ApplicationRunner.RunApplication(null!);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RunApplication_WithValidParameters_ShouldExecuteSuccessfully()
        {
            // Arrange
            var mockService = new Mock<IAnimalApplicationService>();
            var mockOutput = new Mock<ILinePrinter>();

            // Setup to exit immediately
            mockOutput.Setup(x => x.ReadLine()).Returns("0");

            var services = new ServiceCollection();
            services.AddSingleton(mockService.Object);
            services.AddSingleton(mockOutput.Object);

            using var serviceProvider = services.BuildServiceProvider();

            // Act & Assert - Should not throw
            var action = () => ApplicationRunner.RunApplication(serviceProvider);
            action.Should().NotThrow();
        }
    }
}