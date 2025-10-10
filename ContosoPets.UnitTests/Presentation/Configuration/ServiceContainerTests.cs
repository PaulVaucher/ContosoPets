using ContosoPets.Application.Ports;
using ContosoPets.Application.Services;
using ContosoPets.Domain.Services;
using ContosoPets.Presentation.ConsoleApp.Configuration;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ContosoPets.UnitTests.Presentation.Configuration
{
    public class ServiceContainerTests
    {
        [Fact]
        public void ConfigureServices_ShouldReturnValidServiceProvider()
        {
            // Act
            using var serviceProvider = ServiceContainer.ConfigureServices();

            // Assert
            serviceProvider.Should().NotBeNull();
            serviceProvider.Should().BeOfType<ServiceProvider>();
        }

        [Fact]
        public void ConfigureServices_ShouldRegisterAllRequiredServices()
        {
            // Act
            using var serviceProvider = ServiceContainer.ConfigureServices();

            // Assert
            serviceProvider.GetService<IAnimalApplicationService>().Should().NotBeNull();
            serviceProvider.GetService<IAnimalDomainService>().Should().NotBeNull();
            serviceProvider.GetService<IAnimalRepository>().Should().NotBeNull();
            serviceProvider.GetService<ILinePrinter>().Should().NotBeNull();
        }

        [Fact]
        public void ConfigureServices_WhenCalledMultipleTimes_ShouldReturnIndependentProviders()
        {
            // Act
            using var provider1 = ServiceContainer.ConfigureServices();
            using var provider2 = ServiceContainer.ConfigureServices();

            var service1 = provider1.GetRequiredService<IAnimalApplicationService>();
            var service2 = provider2.GetRequiredService<IAnimalApplicationService>();

            // Assert
            provider1.Should().NotBeSameAs(provider2);           

            service1.Should().NotBeSameAs(service2);
        }
    }
}