using ContosoPets.Application.Ports;
using ContosoPets.Domain.Entities;
using ContosoPets.Infrastructure.Entities;
using NHibernate;

namespace ContosoPets.Infrastructure.Repositories
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public AnimalRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public List<Animal> GetAllAnimals()
        {
            using var session = _sessionFactory.OpenSession();
            var nhAnimals = session.Query<NHAnimal>().ToList();
            return nhAnimals.Select(ToDomain).ToList();
        }

        private static Animal ToDomain(NHAnimal nhAnimal)
        {
            return nhAnimal.Species.ToLower() switch
            {
                "dog" => new Dog(
                    nhAnimal.Species,
                    nhAnimal.Id,
                    nhAnimal.Age,
                    nhAnimal.PhysicalDescription,
                    nhAnimal.PersonalityDescription,
                    nhAnimal.Nickname),
                "cat" => new Cat(
                    nhAnimal.Species,
                    nhAnimal.Id,
                    nhAnimal.Age,
                    nhAnimal.PhysicalDescription,
                    nhAnimal.PersonalityDescription,
                    nhAnimal.Nickname),
                _ => throw new InvalidOperationException($"Unknown species: {nhAnimal.Species}")
            };
        }

        public void AddAnimal(Animal animal)
        {
            using var session = _sessionFactory.OpenSession();
            using var transaction = session.BeginTransaction();

            try
            {
                var nhAnimal = ToNHibernateEntity(animal);
                session.Save(nhAnimal);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private static NHAnimal ToNHibernateEntity(Animal animal)
        {
            NHAnimal nhAnimal;
            if (animal is Dog)
            {
                nhAnimal = new NHDog();
            }
            else if (animal is Cat)
            {
                nhAnimal = new NHCat();
            }
            else
            {
                throw new InvalidOperationException($"Unknown animal type: {animal.GetType().Name}");
            }

            // Use reflection to set the protected properties 'Id', 'Species', etc.
            typeof(NHAnimal).GetProperty("Id")?.SetValue(nhAnimal, animal.Id);
            typeof(NHAnimal).GetProperty("Species")?.SetValue(nhAnimal, animal.Species);
            typeof(NHAnimal).GetProperty("Age")?.SetValue(nhAnimal, animal.Age);
            typeof(NHAnimal).GetProperty("PhysicalDescription")?.SetValue(nhAnimal, animal.PhysicalDescription);
            typeof(NHAnimal).GetProperty("PersonalityDescription")?.SetValue(nhAnimal, animal.PersonalityDescription);
            typeof(NHAnimal).GetProperty("Nickname")?.SetValue(nhAnimal, animal.Nickname);

            return nhAnimal;
        }

        public Animal? GetById(string id)
        {
            using var session = _sessionFactory.OpenSession();
            var nhAnimal = session.Get<NHAnimal>(id);
            return nhAnimal != null ? ToDomain(nhAnimal) : null;
        }

        public void UpdateAnimal(Animal animal)
        {
            using var session = _sessionFactory.OpenSession();
            using var transaction = session.BeginTransaction();
            try
            {
                var nhAnimal = ToNHibernateEntity(animal);
                session.Update(nhAnimal);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void DeleteAnimal(Animal animal)
        {
            using var session = _sessionFactory.OpenSession();
            using var transaction = session.BeginTransaction();

            try
            {
                var nhAnimal = session.Get<NHAnimal>(animal.Id);
                if (nhAnimal != null)
                {
                    session.Delete(nhAnimal);
                }
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void SaveChanges()
        {
            //for compatibility with the interface            
        }
    }
}


