using ContosoPets.Domain.Constants;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace ContosoPets.Presentation.ConsoleApp.Configuration
{
    public static class AppConfigurationBuilder
    {
        public static IConfiguration BuildConfiguration()
        {
            var builder = new ConfigurationBuilder();

            var possiblePaths = new[]
            {
                AppContext.BaseDirectory,
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                Directory.GetCurrentDirectory(),
                Path.Combine(Directory.GetCurrentDirectory(), "Presentation", "ConsoleApp"),
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Presentation", "ConsoleApp")
            };

            string? configPath = null;
            foreach (var path in possiblePaths.Where(p => !string.IsNullOrEmpty(p)))
            {
                var settingsFile = Path.Combine(path!, ProgramConstants.AppSettingsFileName);
                if (File.Exists(settingsFile))
                {
                    configPath = path;
                    break;
                }
            }

            if (configPath != null)
            {
                builder.SetBasePath(configPath)
                       .AddJsonFile(ProgramConstants.AppSettingsFileName, optional: false, reloadOnChange: true)
                       .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable(ProgramConstants.EnvironmentVariable) ?? ProgramConstants.DevelopmentEnvironment}.json", optional: true, reloadOnChange: true);
            }
            else
            {
                Console.WriteLine(ProgramConstants.ConfigurationFilesNotFoundMessage);
                builder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = ProgramConstants.DefaultConnectionString,
                    ["NHibernate:ShowSql"] = ProgramConstants.DefaultShowSql,
                    ["NHibernate:FormatSql"] = ProgramConstants.DefaultFormatSql,
                    ["NHibernate:SchemaAction"] = ProgramConstants.DefaultSchemaAction
                });
            }

            return builder.Build();
        }
    }
}
