using ContosoPets.Domain.Constants;
using ContosoPets.Infrastructure.Database;
using ContosoPets.Infrastructure.StartUp;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ContosoPets.UnitTests.Infrastructure.StartUp
{
    public class DatabaseStartupTests
    {
        [Fact]
        public async Task InitializeDatabaseAsync_WithValidServiceProvider_ShouldCallInitializer()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var mockDatabaseLogger = new Mock<ILogger<DatabaseInitializer>>();
            var mockLogger = new Mock<ILogger<object>>();

            var mockInitializer = new DatabaseInitializer(
                mockConfiguration.Object, mockDatabaseLogger.Object);           

            var services = new ServiceCollection();
            services.AddSingleton<DatabaseInitializer>(mockInitializer);
            services.AddSingleton(mockLogger.Object);

            using var serviceProvider = services.BuildServiceProvider();

            // Act & Assert
            var action = async () => await DatabaseStartup.InitializeDatabaseAsync(serviceProvider);
            await action.Should().NotThrowAsync<ArgumentNullException>();            
        }

        [Fact]
        public async Task InitializeDatabaseAsync_ShouldLogStartAndCompletion()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var mockDatabaseLogger = new Mock<ILogger<DatabaseInitializer>>();
            var mockLogger = new Mock<ILogger<object>>();

            var mockInitializer = new DatabaseInitializer(mockConfiguration.Object, mockDatabaseLogger.Object);

            var services = new ServiceCollection();
            services.AddSingleton<DatabaseInitializer>(mockInitializer);
            services.AddSingleton(mockLogger.Object);

            using var serviceProvider = services.BuildServiceProvider();

            // Act
            await DatabaseStartup.InitializeDatabaseAsync(serviceProvider);

            // Assert
            mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Debug),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? string.Empty).Contains(LoggingConstants.DatabaseInitializationStarted)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
            mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? string.Empty).Contains(LoggingConstants.DatabaseInitializationCompleted)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task InitializeDatabaseAsync_WhenInitializeThrows_ShouldPropagateException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<object>>();
            var services = new ServiceCollection();
            services.AddSingleton(mockLogger.Object);
            
            using var serviceProvider = services.BuildServiceProvider();

            // Act & Assert
            var action = async () => await DatabaseStartup.InitializeDatabaseAsync(serviceProvider);
            await action.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
