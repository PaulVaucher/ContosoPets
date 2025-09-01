using ContosoPets.Application.UseCases.Animals;
using ContosoPets.Domain.Entities;
using ContosoPets.Application.Ports;
using ContosoPets.Domain.Constants;
using ContosoPets.Domain.Builders;

namespace ContosoPets.Infrastructure.Services
{
    public class AnimalService(IAnimalRepository repository) : IAnimalService
    {
        private const int MaxPets = 8;
        private const string UnknownAge = "?";
        private const string DefaultValue = "tbd";

        public List<Animal> ListAll()
        {
            return repository.GetAllAnimals().ToList();
        }

        public AddAnimalResult AddNewAnimal(AddAnimalRequest request)
        {
            var animals = ListAll();
            int petCount = animals.Count(animal => !string.IsNullOrEmpty(animal.Id));

            if (petCount >= MaxPets)
            {
                return new AddAnimalResult
                {
                    Success = false,
                    ErrorMessage = AppConstants.PetLimitReachedMessage
                };
            }

            if (request.Species != "dog" && request.Species != "cat")
            {
                return new AddAnimalResult
                {
                    Success = false,
                    ErrorMessage = AppConstants.InvalidSpeciesMessage
                };
            }

            string id = Animal.GenerateId(request.Species, petCount + 1);

            var builder = AnimalBuilder.Builder()
                .WithSpecies(request.Species)
                .WithId(id)
                .WithAge(request.Age)
                .WithPhysicalDescription(request.PhysicalDescription)
                .WithPersonalityDescription(request.PersonalityDescription)
                .WithNickname(request.Nickname);

            try
            {
                var newAnimal = builder.Build();
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
            return ListAll()
                .Where(a => !string.IsNullOrEmpty(a.Id) &&
                            (string.IsNullOrEmpty(a.Age) || a.Age == UnknownAge ||
                             string.IsNullOrEmpty(a.PhysicalDescription) || a.PhysicalDescription == DefaultValue))
                .ToList();
        }

        public void CompleteAgesAndDescriptions(Dictionary<string, (string Age, string PhysicalDescription)> corrections)
        {
            var animals = ListAll();
            bool hasChanges = false;
            foreach (var animal in animals)
            {
                if (corrections.TryGetValue(animal.Id, out var values))
                {
                    if (!string.IsNullOrEmpty(values.Age) && values.Age != UnknownAge)
                    {
                        animal.SetAge(values.Age);
                        hasChanges = true;
                    }
                    if (!string.IsNullOrEmpty(values.PhysicalDescription) && values.PhysicalDescription != DefaultValue)
                    {
                        animal.SetPhysicalDescription(values.PhysicalDescription);
                        hasChanges = true;
                    }
                }
                if (hasChanges)
                {
                    repository.UpdateAnimal(animal);
                    hasChanges = false;
                }
            }            
        }

        public List<Animal> GetAnimalsWithIncompleteNicknameOrPersonality()
        {
            return ListAll()
                .Where(a => !string.IsNullOrEmpty(a.Id) &&
                            (string.IsNullOrEmpty(a.Nickname) || a.Nickname == DefaultValue ||
                             string.IsNullOrEmpty(a.PersonalityDescription) || a.PersonalityDescription == DefaultValue))
                .ToList();
        }

        public void CompleteNicknamesAndPersonality(Dictionary<string, (string Nickname, string Personality)> corrections)
        {
            var animals = ListAll();
            bool hasChanges = false;
            foreach (var animal in animals)
            {
                if (corrections.TryGetValue(animal.Id, out var values))
                {
                    if (!string.IsNullOrEmpty(values.Nickname) && values.Nickname != DefaultValue)
                    {
                        animal.SetNickname(values.Nickname);
                        hasChanges = true;
                    }
                    if (!string.IsNullOrEmpty(values.Personality) && values.Personality != DefaultValue)
                    {
                        animal.SetPersonalityDescription(values.Personality);
                        hasChanges = true;
                    }
                }
                if (hasChanges)
                {
                    repository.UpdateAnimal(animal);
                    hasChanges = false;
                }
            }            
        }

        public Animal? GetAnimalById(string id)
        {
            return ListAll().FirstOrDefault(a => a.Id == id);
        }

        public bool UpdateAnimalAge(string id, string newAge)
        {
            var animal = GetAnimalById(id);
            if (animal != null)
            {
                animal.SetAge(newAge);
                repository.UpdateAnimal(animal);
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
                return true;
            }
            return false;
        }

        public List<Animal> GetAnimalsWithCharacteristic(string species, string characteristic)
        {
            return ListAll()
                .Where(a => a.Species.Equals(species, StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrEmpty(a.Id)
                && (a.PhysicalDescription.Contains(characteristic, StringComparison.OrdinalIgnoreCase) ||
                             a.PersonalityDescription.Contains(characteristic, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }
    }
}