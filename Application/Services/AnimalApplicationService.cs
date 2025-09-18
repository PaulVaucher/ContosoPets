using ContosoPets.Application.Ports;
using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Domain.Constants;
using ContosoPets.Domain.Entities;
using ContosoPets.Domain.Services;

namespace ContosoPets.Application.Services
{
    public class AnimalApplicationService : IAnimalApplicationService
    {
        private readonly IAnimalRepository _repository;
        private readonly IAnimalDomainService _domainService;

        public AnimalApplicationService(IAnimalRepository repository, IAnimalDomainService domainService)
        {
            _repository = repository;
            _domainService = domainService;
        }

        public List<Animal> ListAll()
        {
            return _repository.GetAllAnimals() ?? new List<Animal>();
        }

        public AddAnimalResult AddNewAnimal(AddAnimalRequest request)
        {
            int petCount = _repository.GetAnimalCount();
            var validationError = _domainService.ValidateNewAnimal(request.Species, petCount);

            if (validationError is not null)
            {
                return new AddAnimalResult
                {
                    Success = false,
                    ErrorMessage = validationError
                };
            }            

            try
            {
                var newAnimal = _domainService.BuildAnimal(
                    request.Species,
                    _domainService.GenerateId(request.Species, petCount + 1),
                    request.Age,
                    request.PhysicalDescription,
                    request.PersonalityDescription,
                    request.Nickname);

                _repository.AddAnimal(newAnimal);
                _repository.SaveChanges();

                return new AddAnimalResult
                {
                    Success = true,
                    Animal = newAnimal
                };
            }
            catch (InvalidOperationException ex)
            {
                return new AddAnimalResult
                {
                    Success = false,
                    ErrorMessage = string.Format(AppConstants.AnimalOperationErrorFormat, ex.Message)
                };
            }
        }

        public List<Animal> GetAnimalsWithIncompleteAgeOrDescription()
        {
            return _repository.GetAnimalsWithIncompleteAgeOrDescription();
        }

        public void CompleteAgesAndDescriptions(Dictionary<string, (string Age, string PhysicalDescription)> corrections)
        {
            var animalIds = corrections.Keys.ToList();
            var animals = animalIds.Select(id => _repository.GetById(id))
                .Where(animal => animal != null)
                .Cast<Animal>()
                .ToList();

            bool hasChanges = false;

            foreach (var animal in animals)
            {
                if (corrections.TryGetValue(animal.Id, out var values))
                {
                    bool animalModified = false;

                    if (!string.IsNullOrEmpty(values.Age))
                    {
                        animal.SetAge(values.Age);
                        animalModified = true;
                    }
                    if (!string.IsNullOrEmpty(values.PhysicalDescription))
                    {
                        animal.SetPhysicalDescription(values.PhysicalDescription);
                        animalModified = true;
                    }

                    if (animalModified)
                    {
                        _repository.UpdateAnimal(animal);
                        hasChanges = true;
                    }
                }
            }
            if (hasChanges)
            {
                _repository.SaveChanges();
            }
        }

        public List<Animal> GetAnimalsWithIncompleteNicknameOrPersonality()
        {
            return _repository.GetAnimalsWithIncompleteNicknameOrPersonality();
        }

        public void CompleteNicknamesAndPersonality(Dictionary<string, (string Nickname, string Personality)> corrections)
        {
            var animalIds = corrections.Keys.ToList();
            var animals = animalIds.Select(id => _repository.GetById(id))
                .Where(animal => animal != null)
                .Cast<Animal>()
                .ToList();

            bool hasChanges = false;

            foreach (var animal in animals)
            {
                if (corrections.TryGetValue(animal.Id, out var values))
                {
                    bool animalModified = false;
                    if (!string.IsNullOrEmpty(values.Nickname))
                    {
                        animal.SetNickname(values.Nickname);
                        animalModified = true;
                    }
                    if (!string.IsNullOrEmpty(values.Personality))
                    {
                        animal.SetPersonalityDescription(values.Personality);
                        animalModified = true;
                    }
                    if (animalModified)
                    {
                        _repository.UpdateAnimal(animal);
                        hasChanges = true;
                    }
                }
            }
            if (hasChanges)
            {
                _repository.SaveChanges();
            }
        }

        public Animal? GetAnimalById(string id)
        {
            return _repository.GetById(id);
        }

        public bool UpdateAnimalAge(string id, string newAge)
        {
            var animal = GetAnimalById(id);
            if (animal != null)
            {
                animal.SetAge(newAge);
                _repository.UpdateAnimal(animal);
                _repository.SaveChanges();
                return true;
            }
            return false;
        }

        public bool UpdateAnimalPersonality(string id, string newPersonality)
        {
            var animal = GetAnimalById(id);
            if (animal != null)
            {
                animal.SetPersonalityDescription(newPersonality);
                _repository.UpdateAnimal(animal);
                _repository.SaveChanges();
                return true;
            }
            return false;
        }

        public List<Animal> GetAnimalsWithCharacteristic(string species, string characteristic)
        {
            if (string.IsNullOrEmpty(species) || string.IsNullOrEmpty(characteristic))
                return [];

            return _repository.GetAnimalsWithCharacteristic(species, characteristic);
        }
    }
}
