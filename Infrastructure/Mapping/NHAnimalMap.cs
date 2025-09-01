using ContosoPets.Infrastructure.Entities;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace ContosoPets.Infrastructure.Mappings
{
    public class NHAnimalMap : ClassMapping<NHAnimal>
    {
        public NHAnimalMap()
        {
            Table("animals");

            Id(x => x.Id, m =>
            {
                m.Column("id");
                m.Type(NHibernateUtil.String);
                m.Length(10);
                m.Generator(Generators.Assigned);                
            });

            Property(x => x.Species, m =>
            {
                m.Column("species");
                m.Type(NHibernateUtil.String);
                m.Length(50);
                m.NotNullable(true);
            });

            Property(x => x.Age, m =>
            {
                m.Column("age");
                m.Type(NHibernateUtil.String);
                m.Length(10);
            });

            Property(x => x.PhysicalDescription, m =>
            {
                m.Column("physical_description");
                m.Type(NHibernateUtil.StringClob);
            });

            Property(x => x.PersonalityDescription, m =>
            {
                m.Column("personality_description");
                m.Type(NHibernateUtil.StringClob);
            });

            Property(x => x.Nickname, m =>
            {
                m.Column("nickname");
                m.Type(NHibernateUtil.String);
                m.Length(50);
            });

            // Column-based discrimination for inheritance
            Discriminator(m =>
            {
                m.Column("discriminator");
                m.Type(NHibernateUtil.String);
                m.Length(20);
            });
        }
    }
}