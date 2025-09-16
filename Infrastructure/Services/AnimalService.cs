using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Domain.Entities;
using ContosoPets.Application.Ports;
using ContosoPets.Domain.Constants;
using ContosoPets.Domain.Builders;

namespace ContosoPets.Infrastructure.Services
{
    public class AnimalService(IAnimalRepository repository) : IAnimalService
    {
        private static readonly HashSet<string> SupportedSpecies = new(StringComparer.OrdinalIgnoreCase)
        {
            "dog",
            "cat"
        };

        public List<Animal> ListAll()
        {
            return repository.GetAllAnimals() ?? new List<Animal>();
        }

        private string? ValidateNewAnimal(AddAnimalRequest request, int petCount)
        {
            if (petCount >= AppConstants.MaxPets)
            {
                return AppConstants.PetLimitReachedMessage;
            }
            if (string.IsNullOrWhiteSpace(request.Species) || !SupportedSpecies.Contains(request.Species))
            {
                return AppConstants.InvalidSpeciesMessage;
            }
            return null;
        }

        private string GenerateId(string species, int nextIndex) =>
            Animal.GenerateId(species, nextIndex);

        private Animal BuildAnimal(AddAnimalRequest request, string id)
        {
            return AnimalBuilder.Builder()
                .WithSpecies(request.Species)
                .WithId(id)
                .WithAge(request.Age)
                .WithPhysicalDescription(request.PhysicalDescription)
                .WithPersonalityDescription(request.PersonalityDescription)
                .WithNickname(request.Nickname)
                .Build();
        }

        public AddAnimalResult AddNewAnimal(AddAnimalRequest request)
        {
            int petCount = repository.GetAnimalCount();
            var validationError = ValidateNewAnimal(request, petCount);

            if (validationError is not null)
            {
                return new AddAnimalResult
                {
                    Success = false,
                    ErrorMessage = validationError
                };
            }

            string id = GenerateId(request.Species, petCount + 1);

            try
            {
                var newAnimal = BuildAnimal(request, id);
                repository.AddAnimal(newAnimal);
                repository.SaveChanges();

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
            return repository.GetAnimalsWithIncompleteAgeOrDescription();
        }

        public void CompleteAgesAndDescriptions(Dictionary<string, (string Age, string PhysicalDescription)> corrections)
        {
            var animalIds = corrections.Keys.ToList();
            var animals = animalIds.Select(id => repository.GetById(id))
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
                        repository.UpdateAnimal(animal);
                        hasChanges = true;
                    }
                }
            }
            if (hasChanges)
            {
                repository.SaveChanges();
            }
        }

        public List<Animal> GetAnimalsWithIncompleteNicknameOrPersonality()
        {
            return repository.GetAnimalsWithIncompleteNicknameOrPersonality();
        }

        public void CompleteNicknamesAndPersonality(Dictionary<string, (string Nickname, string Personality)> corrections)
        {
            var animalIds = corrections.Keys.ToList();
            var animals = animalIds.Select(id => repository.GetById(id))
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
                        repository.UpdateAnimal(animal);
                        hasChanges = true;
                    }
                }
            }
            if (hasChanges)
            {
                repository.SaveChanges();
            }
        }

        public Animal? GetAnimalById(string id)
        {
            return repository.GetById(id);
        }

        public bool UpdateAnimalAge(string id, string newAge)
        {
            var animal = GetAnimalById(id);
            if (animal != null)
            {
                animal.SetAge(newAge);
                repository.UpdateAnimal(animal);
                repository.SaveChanges();
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
                repository.UpdateAnimal(animal);
                repository.SaveChanges();
                return true;
            }
            return false;
        }

        public List<Animal> GetAnimalsWithCharacteristic(string species, string characteristic)
        {
            if (string.IsNullOrEmpty(species) || string.IsNullOrEmpty(characteristic))
                return [];

            return repository.GetAnimalsWithCharacteristic(species, characteristic);
        }
    }
}