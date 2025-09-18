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
        
        public List<Animal> GetAnimalsWithIncompleteNicknameOrPersonality()
        {
            return _repository.GetAnimalsWithIncompleteNicknameOrPersonality();
        }

        private void UpdateAnimalsFromCorrections<T>(
            Dictionary<string, T> corrections,
            Action<Animal, T> updateAction)
        {
            if (corrections == null || !corrections.Any())
            {
                throw new ArgumentException(AppConstants.NoCorrectionsProvidedMessage);
            }

            var animalIds = corrections.Keys.ToList();
            var animals = animalIds
                .Select(id => _repository.GetById(id))
                .Where(animal => animal != null)
                .Cast<Animal>()
                .ToList();

            if (!animals.Any())
            {
                throw new InvalidOperationException(AppConstants.NoAnimalsFoundWithIdsMessage);
            }

            bool hasChanges = false;

            foreach (var animal in animals)
            {
                if (corrections.TryGetValue(animal.Id, out var value))
                {
                    try
                    {
                        updateAction(animal, value);
                        _repository.UpdateAnimal(animal);
                        hasChanges = true;
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(
                            string.Format(AppConstants.AnimalOperationErrorFormat, ex.Message));
                    }
                }
            }

            if (hasChanges)
            {
                try
                {
                    _repository.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        string.Format(AppConstants.AnimalOperationErrorFormat, ex.Message));
                }
            }
        }

        public void CompleteAgesAndDescriptions(Dictionary<string, (string Age, string PhysicalDescription)> corrections)
        {
            UpdateAnimalsFromCorrections(corrections, (animal, values) =>
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

                if (!animalModified)
                {
                    throw new InvalidOperationException(AppConstants.NoValidModificationsMessage);
                }
            });
        }

        public void CompleteNicknamesAndPersonality(Dictionary<string, (string Nickname, string Personality)> corrections)
        {
            UpdateAnimalsFromCorrections(corrections, (animal, values) =>
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

                if (!animalModified)
                {
                    throw new InvalidOperationException(AppConstants.NoValidModificationsMessage);
                }
            });
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
