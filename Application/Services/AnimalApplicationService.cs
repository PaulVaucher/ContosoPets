using ContosoPets.Application.Ports;
using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Domain.Constants;
using ContosoPets.Domain.Entities;
using ContosoPets.Domain.Services;
using ContosoPets.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ContosoPets.Application.Services
{
    public class AnimalApplicationService : IAnimalApplicationService
    {
        private readonly ILogger<AnimalApplicationService> _logger;
        private readonly IAnimalRepository _repository;
        private readonly IAnimalDomainService _domainService;

        public AnimalApplicationService(ILogger<AnimalApplicationService> logger,
            IAnimalRepository repository, IAnimalDomainService domainService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
        }

        public List<Animal> ListAll()
        {
            using var scope = _logger.BeginScope("Operation={Operation}", LoggingConstants.OperationListAllAnimals);
            var stopwatch = Stopwatch.StartNew();

            _logger.LogDebug(LoggingConstants.ServiceOperationStarted, LoggingConstants.OperationTypeListAll, LoggingConstants.EntityTypeAnimal);

                var animals = _repository.GetAllAnimals() ?? new List<Animal>();
                stopwatch.Stop();

                _logger.LogInformation(LoggingConstants.ServiceOperationCompleted,
                    LoggingConstants.OperationTypeListAll, LoggingConstants.EntityTypeAnimal, stopwatch.ElapsedMilliseconds);
                _logger.LogDebug(LoggingConstants.SearchOperationCompleted, animals.Count);

                return animals;
            }

        public AddAnimalResult AddNewAnimal(AddAnimalRequest request)
        {
            if (request == null)
            {            
                _logger.LogWarning(LoggingConstants.ValidationNullRequest);
                throw new ArgumentNullException(nameof(request));
            }

            using var scope = _logger.BeginScope("Operation={Operation}, Species={Species}",
                LoggingConstants.OperationAddNewAnimal, request.Species);
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation(LoggingConstants.AnimalCreationStarted, request.Species);

            try
            {            
                int petCount = _repository.GetAnimalCount();
                _logger.LogDebug(LoggingConstants.CurrentPetCount, petCount);

                var validationError = _domainService.ValidateNewAnimal(request.Species, petCount);

                if (validationError is not null)
                {
                    _logger.LogWarning(LoggingConstants.AnimalValidationFailed, validationError);
                    return new AddAnimalResult
                    {
                        Success = false,
                        ErrorMessage = validationError
                    };
                }
                        
                var newAnimal = _domainService.BuildAnimal(
                    request.Species,
                    _domainService.GenerateId(request.Species, petCount + 1),
                    request.Age,
                    request.PhysicalDescription,
                    request.PersonalityDescription,
                    request.Nickname);

                _repository.AddAnimal(newAnimal);
                stopwatch.Stop();

                _logger.LogInformation(LoggingConstants.AnimalCreationCompleted,
                    newAnimal.Id, newAnimal.Species);
                _logger.LogDebug(LoggingConstants.PerformanceMetric, LoggingConstants.OperationAddNewAnimal, stopwatch.ElapsedMilliseconds);

                return new AddAnimalResult
                {
                    Success = true,
                    Animal = newAnimal
                };
            }
            catch (InvalidOperationException ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, LoggingConstants.ServiceOperationFailed,
                    LoggingConstants.OperationAddNewAnimal, LoggingConstants.EntityTypeAnimal, ex.Message);

                return new AddAnimalResult
                {
                    Success = false,
                    ErrorMessage = string.Format(AppConstants.AnimalOperationErrorFormat, ex.Message)
                };
            }
        }

        public List<Animal> GetAnimalsWithIncompleteAgeOrDescription()
        {
            using var scope = _logger.BeginScope("Operation={Operation}", LoggingConstants.OperationGetIncompleteAgeOrDescription);

            _logger.LogDebug(LoggingConstants.FilteringIncompleteData, LoggingConstants.DataTypeAgeOrDescription);

            var result = _repository.GetAnimalsWithIncompleteAgeOrDescription();

            _logger.LogInformation(LoggingConstants.SearchOperationCompleted, result.Count);

            return result;
        }

        public List<Animal> GetAnimalsWithIncompleteNicknameOrPersonality()
        {
            using var scope = _logger.BeginScope("Operation={Operation}", LoggingConstants.OperationGetIncompleteNicknameOrPersonality);

            _logger.LogDebug(LoggingConstants.FilteringIncompleteData, LoggingConstants.DataTypeNicknameOrPersonality);

            var result = _repository.GetAnimalsWithIncompleteNicknameOrPersonality();

            _logger.LogInformation(LoggingConstants.SearchOperationCompleted, result.Count);

            return result;
        }

        private void UpdateAnimalsFromCorrections<T>(
            Dictionary<AnimalId, T> corrections,
            Action<Animal, T> updateAction)
        {
            if (corrections == null || corrections.Count == 0)
            {
                _logger.LogWarning(LoggingConstants.ValidationEmptyCorrections);
                throw new ArgumentException(AppConstants.NoCorrectionsProvidedMessage);
            }

            var animalIds = corrections.Keys.ToList();
            _logger.LogDebug(LoggingConstants.UpdatingAnimalsWithCorrections, animalIds.Count);

            var animals = animalIds
                .Select(id => _repository.GetById(id.Value))
                .Where(animal => animal != null)
                .Cast<Animal>()
                .ToList();

            if (!animals.Any())
            {
                var idsString = string.Join(", ", animalIds.Select(id => id.Value));
                _logger.LogWarning(LoggingConstants.AnimalNotFound, idsString);
                throw new InvalidOperationException(AppConstants.NoAnimalsFoundWithIdsMessage);
            }

            foreach (var animal in animals)
            {
                if (corrections.TryGetValue(animal.Id, out var value))
                {
                    try
                    {
                        updateAction(animal, value);
                        _repository.UpdateAnimal(animal);
                        _logger.LogDebug(LoggingConstants.AnimalUpdatedSuccessfully, animal.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, LoggingConstants.AnimalUpdateFailed, animal.Id, ex.Message);
                        throw new InvalidOperationException(
                            string.Format(AppConstants.AnimalUpdateErrorFormat, animal.Id, ex.Message));
                    }
                }
            }            
        }

        public void CompleteAgesAndDescriptions(Dictionary<AnimalId, (string Age, string PhysicalDescription)> corrections)
        {
            using var scope = _logger.BeginScope("Operation={Operation}", LoggingConstants.OperationCompleteAgesAndDescriptions);
            _logger.LogInformation(LoggingConstants.BatchUpdateStarted, LoggingConstants.UpdateTypeAgesAndDescriptions);

            UpdateAnimalsFromCorrections(corrections, (animal, values) =>
            {
                bool animalModified = false;

                if (!string.IsNullOrEmpty(values.Age))
                {
                    var oldAge = animal.Age;
                    animal.SetAge(values.Age);
                    _logger.LogDebug(LoggingConstants.ObjectStateChange,
                        LoggingConstants.EntityTypeAnimal, animal.Id.Value, LoggingConstants.PropertyAge, oldAge, values.Age);
                    animalModified = true;
                }
                if (!string.IsNullOrEmpty(values.PhysicalDescription))
                {
                    var oldDescription = animal.PhysicalDescription;
                    animal.SetPhysicalDescription(values.PhysicalDescription);
                    _logger.LogDebug(LoggingConstants.ObjectStateChange,
                        LoggingConstants.EntityTypeAnimal, animal.Id.Value, LoggingConstants.PropertyPhysicalDescription, oldDescription, values.PhysicalDescription);
                    animalModified = true;
                }

                if (!animalModified)
                {
                    _logger.LogWarning(LoggingConstants.NoValidModificationsForAnimal, animal.Id);
                    throw new InvalidOperationException(AppConstants.NoValidModificationsMessage);
                }
            });

            _logger.LogInformation(LoggingConstants.BatchUpdateCompleted, LoggingConstants.UpdateTypeAgesAndDescriptions);
        }

        public void CompleteNicknamesAndPersonality(Dictionary<AnimalId, (string Nickname, string Personality)> corrections)
        {
            UpdateAnimalsFromCorrections(corrections, (animal, values) =>
            {
                using var scope = _logger.BeginScope("Operation={Operation}", LoggingConstants.OperationCompleteNicknamesAndPersonality);
                _logger.LogInformation(LoggingConstants.BatchUpdateStarted, LoggingConstants.UpdateTypeNicknamesAndPersonality);

                bool animalModified = false;

                if (!string.IsNullOrEmpty(values.Nickname))
                {
                    var oldNickname = animal.Nickname;
                    animal.SetNickname(values.Nickname);
                    _logger.LogDebug(LoggingConstants.ObjectStateChange,
                        LoggingConstants.EntityTypeAnimal, animal.Id.Value, LoggingConstants.PropertyNickname, oldNickname, values.Nickname);
                    animalModified = true;
                }
                if (!string.IsNullOrEmpty(values.Personality))
                {
                    var oldPersonality = animal.PersonalityDescription;
                    animal.SetPersonalityDescription(values.Personality);
                    _logger.LogDebug(LoggingConstants.ObjectStateChange,
                        LoggingConstants.EntityTypeAnimal, animal.Id.Value, LoggingConstants.PropertyPersonalityDescription, oldPersonality, values.Personality);
                    animalModified = true;
                }

                if (!animalModified)
                {
                    _logger.LogWarning(LoggingConstants.NoValidModificationsForAnimal, animal.Id);
                    throw new InvalidOperationException(AppConstants.NoValidModificationsMessage);
                }

                _logger.LogInformation(LoggingConstants.BatchUpdateCompleted, LoggingConstants.UpdateTypeNicknamesAndPersonality);
            });
        }

        public Animal? GetAnimalById(string id)
        {
            using var scope = _logger.BeginScope("AnimalId={AnimalId}", id);

            _logger.LogDebug(LoggingConstants.RetrievingAnimalById, id);

            var animal = _repository.GetById(id);

            if (animal == null)
            {
                _logger.LogInformation(LoggingConstants.AnimalNotFound, id);
            }
            else
            {
                _logger.LogDebug(LoggingConstants.AnimalFoundWithDetails, animal.Species, animal.Nickname);
            }

            return animal;
        }

        public bool UpdateAnimalAge(string id, string newAge)
        {
            using var scope = _logger.BeginScope("Operation={Operation} AnimalId={AnimalId}", LoggingConstants.OperationUpdateAge, id);


            var animal = GetAnimalById(id);
            if (animal != null)
            {
                var oldAge = animal.Age;
                animal.SetAge(newAge);
                _repository.UpdateAnimal(animal);

                _logger.LogInformation(LoggingConstants.ObjectStateChange,
                    LoggingConstants.EntityTypeAnimal, id, LoggingConstants.PropertyAge, oldAge, newAge);            

                return true;
            }
            return false;
        }

        public bool UpdateAnimalPersonality(string id, string newPersonality)
        {
            using var scope = _logger.BeginScope("Operation={Operation} AnimalId={AnimalId}", LoggingConstants.OperationUpdatePersonality, id);


            var animal = GetAnimalById(id);
            if (animal != null)
            {
                var oldPersonality = animal.PersonalityDescription;
                animal.SetPersonalityDescription(newPersonality);
                _repository.UpdateAnimal(animal);

                _logger.LogInformation(LoggingConstants.ObjectStateChange,
                    LoggingConstants.EntityTypeAnimal, id, LoggingConstants.PropertyPersonalityDescription, oldPersonality, newPersonality);
                return true;
            }
            return false;
        }

        public List<Animal> GetAnimalsWithCharacteristic(string species, string characteristic)
        {
            if (string.IsNullOrEmpty(species) || string.IsNullOrEmpty(characteristic))
            {
            _logger.LogWarning(LoggingConstants.SearchWithEmptyParameters, species, characteristic);
            return [];
            }

            using var scope = _logger.BeginScope("Operation={Operation} Species={Species} Characteristic={Characteristic}",
                LoggingConstants.OperationSearchByCharacteristic, species, characteristic);

            _logger.LogInformation(LoggingConstants.SearchOperationStarted, species, characteristic);

            var result = _repository.GetAnimalsWithCharacteristic(species, characteristic);

            _logger.LogInformation(LoggingConstants.SearchOperationCompleted, result.Count);

            return result;
        }
    }
}