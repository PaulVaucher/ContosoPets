namespace ContosoPets.Domain.Constants
{
    public abstract class ProgramConstants
    {
        // Configuration messages
        public const string ConfigurationFilesNotFoundMessage = "Configuration files not found, using default configuration.";

        // Default configuration values
        public const string DefaultConnectionString = "Host=localhost;Port=5432;Database=contoso_pets;Username=postgres;Password=postgres";
        public const string DefaultShowSql = "true";
        public const string DefaultFormatSql = "true";
        public const string DefaultSchemaAction = "create-drop";

        // Configuration file names
        public const string AppSettingsFileName = "appsettings.json";
        public const string DevelopmentEnvironment = "Development";

        // Configuration keys
        public const string ConnectionStringKey = "ConnectionStrings:DefaultConnection";
        public const string ShowSqlKey = "NHibernate:ShowSql";
        public const string FormatSqlKey = "NHibernate:FormatSql";
        public const string SchemaActionKey = "NHibernate:SchemaAction";
        public const string EnvironmentVariable = "DOTNET_ENVIRONMENT";
    }
}