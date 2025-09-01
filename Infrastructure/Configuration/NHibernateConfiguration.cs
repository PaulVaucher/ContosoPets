using ContosoPets.Infrastructure.Mappings;
using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;

namespace ContosoPets.Infrastructure.Configuration
{
    public class NHibernateConfiguration
    {
        private readonly IConfiguration _configuration;

        public NHibernateConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ISessionFactory CreateSessionFactory()
        {
            var configuration = new NHibernate.Cfg.Configuration();

            // Configuration de base
            configuration.DataBaseIntegration(db =>
            {
                db.Dialect<PostgreSQL83Dialect>();
                db.Driver<NpgsqlDriver>();
                db.ConnectionString = _configuration.GetConnectionString("DefaultConnection");
                db.LogSqlInConsole = bool.Parse(_configuration["NHibernate:ShowSql"] ?? "false");
                db.LogFormattedSql = bool.Parse(_configuration["NHibernate:FormatSql"] ?? "false");
                db.AutoCommentSql = true;
                db.SchemaAction = SchemaAutoAction.Update;
            });

            // Ajout des mappings
            var mapper = new ModelMapper();
            mapper.AddMapping<NHAnimalMap>();
            mapper.AddMapping<NHDogMap>();
            mapper.AddMapping<NHCatMap>();

            var hbmMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
            configuration.AddMapping(hbmMapping);

            return configuration.BuildSessionFactory();
        }
    }
}