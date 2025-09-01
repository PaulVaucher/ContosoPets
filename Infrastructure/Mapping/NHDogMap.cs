using ContosoPets.Infrastructure.Entities;
using NHibernate.Mapping.ByCode.Conformist;

namespace ContosoPets.Infrastructure.Mappings
{
    public class NHDogMap : SubclassMapping<NHDog>
    {
        public NHDogMap()
        {
            DiscriminatorValue("dog");
        }
    }
}