using ContosoPets.Infrastructure.Entities;
using NHibernate.Mapping.ByCode.Conformist;

namespace ContosoPets.Infrastructure.Mappings
{
    public class NHCatMap : SubclassMapping<NHCat>
    {
        public NHCatMap()
        {
            DiscriminatorValue("cat");
        }
    }
}