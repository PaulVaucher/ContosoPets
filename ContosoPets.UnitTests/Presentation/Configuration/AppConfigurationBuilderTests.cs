using ContosoPets.Presentation.ConsoleApp.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ContosoPets.UnitTests.Presentation.Configuration
{
    public class AppConfigurationBuilderTests
    {
        [Fact]
        public void BuildConfiguration_ShouldReturnValidConfiguration()
        {            
            // Act
            var configuration = AppConfigurationBuilder.BuildConfiguration();

            // Assert
            configuration.Should().NotBeNull();
            configuration.Should().BeAssignableTo<IConfiguration>();
        }

        [Fact]
        public void BuildConfiguration_WhenNoConfigFileExists_ShouldUseDefaultValues()
        {
            // Arrange
            var configuration = AppConfigurationBuilder.BuildConfiguration();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Assert
            configuration.Should().NotBeNull();

            connectionString.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void BuildConfiguration_ShouldBeConsistentAcrossMultipleCalls()
        {
            // Act
            var config1 = AppConfigurationBuilder.BuildConfiguration();
            var config2 = AppConfigurationBuilder.BuildConfiguration();
            
            // Assert
            var connectionString1 = config1.GetConnectionString("DefaultConnection");
            var connectionString2 = config2.GetConnectionString("DefaultConnection");

            connectionString1.Should().Be(connectionString2);
        }
    }
}
