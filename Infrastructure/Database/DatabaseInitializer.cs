using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ContosoPets.Infrastructure.Database
{
    public class DatabaseInitializer
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(IConfiguration configuration, ILogger<DatabaseInitializer> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InitializeDatabaseAsync()
        {
            try
            {
                var adminConnectionString = GetAdminConnectionString();
                await CreateDatabaseIfNotExistsAsync(adminConnectionString);

                var appConnectionString = GetConnectionString();
                await CreateTableAsync(appConnectionString);
                await SeedDataAsync(appConnectionString);

                _logger.LogInformation("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during database initialization.");                
            }
        }

        private async Task CreateDatabaseIfNotExistsAsync(string connectionString)
        {
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var checkDbCommand = new NpgsqlCommand(
                "SELECT 1 FROM pg_database WHERE datname = 'contoso_pets'", connection);

            var exists = await checkDbCommand.ExecuteScalarAsync();

            if (exists == null)
            {
                var createDbCommand = new NpgsqlCommand(
                    "CREATE DATABASE contoso_pets WITH OWNER = postgres ENCODING = 'UTF8'", connection);
                await createDbCommand.ExecuteNonQueryAsync();
                _logger.LogInformation("Database 'contoso_pets' created.");
            }
            else
            {
                _logger.LogInformation("Database 'contoso_pets' already exists.");
            }
        }

        private async Task CreateTableAsync(string connectionString)
        {
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var createTableCommand = @"
            CREATE TABLE IF NOT EXISTS animals (
                id VARCHAR(10) PRIMARY KEY,
                species VARCHAR(50) NOT NULL,
                age VARCHAR(10),
                physical_description TEXT,
                personality_description TEXT,
                nickname VARCHAR(50),
                discriminator VARCHAR(20) NOT NULL,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );

            CREATE INDEX IF NOT EXISTS idx_species ON animals(species);
            CREATE INDEX IF NOT EXISTS idx_discriminator ON animals(discriminator);
            ";

            var command = new NpgsqlCommand(createTableCommand, connection);
            await command.ExecuteNonQueryAsync();
            _logger.LogInformation("Table 'animals' ensured.");
        }

        private async Task SeedDataAsync(string connectionString)
        {
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var checkDataCommand = new NpgsqlCommand("SELECT COUNT(*) FROM animals", connection);
            var count = (long)(await checkDataCommand.ExecuteScalarAsync() ?? 0L);

            if (count == 0)
            {
                var seedSql = @"
                INSERT INTO animals (id, species, age, physical_description, personality_description, nickname, discriminator)
                    VALUES 
                    ('d1', 'dog', '2', 'medium sized cream colored female golden retriever weighing about 65 pounds. housebroken.', 'loves to have her belly rubbed and likes to chase her tail. gives lots of kisses.', 'lola', 'dog'),
                    ('d2', 'dog', '9', 'large reddish-brown male golden retriever weighing about 85 pounds. housebroken.', 'loves to have his ears rubbed when he greets you at the door, or at any time! loves to lean-in and give doggy hugs.', 'loki', 'dog'),
                    ('c3', 'cat', '1', 'small white female weighing about 8 pounds. litter box trained.', 'friendly', 'Puss', 'cat'),
                    ('c4', 'cat', '?', '', '', '', 'cat');
                ";

                var command = new NpgsqlCommand(seedSql, connection);
                await command.ExecuteNonQueryAsync();
                _logger.LogInformation("Seed data inserted into 'animals' table.");
            }
            else
            {
                _logger.LogInformation("'animals' table already has data. Skipping seeding.");
            }
        }

        private string GetAdminConnectionString()
        {
            var connCtring = _configuration.GetConnectionString("DefaultConnection") ?? "";
            var builder = new NpgsqlConnectionStringBuilder(connCtring)
            {
                Database = "postgres"
            };
            return builder.ConnectionString;
        }

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection") ?? "";
        }
    }
}