using ContosoPets.Application.Ports;
using ContosoPets.Domain.Constants;
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

        public int GetAnimalCount()
        {
            using var session = _sessionFactory.OpenSession();
            return session.Query<NHAnimal>()
                .Where(a => a.Id != null && a.Id != "")
                .Count();
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
        
        public List<Animal> GetAnimalsWithIncompleteAgeOrDescription()
        {
            using var session = _sessionFactory.OpenSession();
            var nhAnimals = session.Query<NHAnimal>()
                .Where(a => a.Id != null && a.Id != "" &&
                            ((a.Nickname == null || a.Nickname == "" || a.Nickname == AppConstants.DefaultValue) ||
                             (a.PersonalityDescription == null || a.PersonalityDescription == "" || a.PersonalityDescription == AppConstants.DefaultValue)))
                .ToList();

            return nhAnimals.Select(ToDomain).ToList();
        }

        public List<Animal> GetAnimalsWithIncompleteNicknameOrPersonality()
        {
            using var session = _sessionFactory.OpenSession();
            var nhAnimals = session.Query<NHAnimal>()
                .Where(a => a.Id != null && a.Id != "" &&
                            ((a.Nickname == null || a.Nickname == "" || a.Nickname == AppConstants.DefaultValue) ||
                             (a.PersonalityDescription == null || a.PersonalityDescription == "" || a.PersonalityDescription == AppConstants.DefaultValue)))
                .ToList();

            return nhAnimals.Select(ToDomain).ToList();
        }

        public List<Animal> GetAnimalsWithCharacteristic(string species, string characteristic)
        {
            using var session = _sessionFactory.OpenSession();

            var lowerSpecies = species.ToLower();
            var lowerCharacteristic = characteristic.ToLower();

            var nhAnimals = session.Query<NHAnimal>()
                .Where(a => a.Species.ToLower() == lowerSpecies
                && a.Id != null && a.Id != ""
                && (a.PhysicalDescription.ToLower().Contains(lowerCharacteristic) ||
                    a.PersonalityDescription.ToLower().Contains(lowerCharacteristic)))
                .ToList();

            return nhAnimals.Select(ToDomain).ToList();
        }

        public void SaveChanges()
        {
            //for compatibility with the interface            
        }
    }
}


