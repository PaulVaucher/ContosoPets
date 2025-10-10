using ContosoPets.Application.Ports;
using ContosoPets.Application.Services;
using ContosoPets.Domain.Services;
using ContosoPets.Infrastructure.Database;
using ContosoPets.Presentation.ConsoleApp.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ContosoPets.UnitTests.Presentation.Configuration
{
    public class ServiceContainerTests
    {
        [Fact]
        public void ConfigureServices_ShouldReturnValidServiceProvider()
        {
            // Act
            var serviceProvider = ServiceContainer.ConfigureServices();

            // Assert
            serviceProvider.Should().NotBeNull();
            serviceProvider.Should().BeOfType<ServiceProvider>();
        }

        [Fact]
        public void ConfigureServices_ShouldRegisterAllRequiredServices()
        {
            // Act
            var serviceProvider = ServiceContainer.ConfigureServices();

            // Assert
            serviceProvider.GetService<IConfiguration>().Should().NotBeNull();
            serviceProvider.GetService<IAnimalApplicationService>().Should().NotBeNull();
            serviceProvider.GetService<IAnimalDomainService>().Should().NotBeNull();
            serviceProvider.GetService<IAnimalRepository>().Should().NotBeNull();
            serviceProvider.GetService<ILinePrinter>().Should().NotBeNull();
            serviceProvider.GetService<DatabaseInitializer>().Should().NotBeNull();
        }

        [Fact]
        public void ConfigureServices_ShouldRegisterLogging()
        {
            // Act
            var serviceProvider = ServiceContainer.ConfigureServices();

            // Assert
            var logger = serviceProvider.GetService<ILogger<object>>();
            logger.Should().NotBeNull();
        }

        [Fact]
        public void ConfigureServices_WhenCalledMultipleTimes_ShouldReturnIndependentProviders()
        {
            // Act
            var provider1 = ServiceContainer.ConfigureServices();
            var provider2 = ServiceContainer.ConfigureServices();

            var service1 = provider1.GetRequiredService<IAnimalApplicationService>();
            var service2 = provider2.GetRequiredService<IAnimalApplicationService>();

            // Assert
            provider1.Should().NotBeSameAs(provider2);

            service1.Should().NotBeSameAs(service2);
        }
    }
}
