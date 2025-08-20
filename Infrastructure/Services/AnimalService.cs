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
        private const string CanYes = "y";
        private const string CanNo = "n";
        private const string UnknownAge = "?";
        private const string DefaultValue = "tbd";

        public List<Animal> ListAll()
        {
            return repository.GetAllAnimals().ToList();
        }

        public void AddNewAnimal()
        {
            var animals = ListAll();
            int petCount = animals.Count(animal => !string.IsNullOrEmpty(animal.Id));

            if (petCount >= MaxPets)
            {
                Console.WriteLine(AppConstants.PetLimitReachedMessage);
                return;
            }
            Console.WriteLine(string.Format(AppConstants.CurrentPetsStatusFormat, petCount, MaxPets - petCount));
            string anotherPet = CanYes;

            while (anotherPet == CanYes && petCount < MaxPets)
            {
                try
                {
                    string species = GetValidSpecies();
                    string id = Animal.GenerateId(species, petCount + 1);

                    var builder = AnimalBuilder.Builder()
                        .WithSpecies(species)
                        .WithId(id)
                        .WithAge(GetValidAge(id))
                        .WithPhysicalDescription(GetPhysicalDescription(id))
                        .WithPersonalityDescription(GetPersonalityDescription(id))
                        .WithNickname(GetNickname(id));

                    var newAnimal = builder.Build();
                    repository.AddAnimal(newAnimal);
                    petCount++;

                    repository.SaveChanges();

                    if (petCount < MaxPets)
                    {
                        Console.WriteLine(AppConstants.AddAnotherPetPrompt);
                        anotherPet = GetYesNoInput();
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(string.Format(AppConstants.AnimalOperationErrorFormat, ex.Message));
                }
            }                
        }

        private static string GetValidSpecies()
        {
            Console.Write(AppConstants.EnterSpeciesPrompt);
            string input = Console.ReadLine()?.ToLower() ?? "";
            while (input != "dog" && input != "cat")
            {
                Console.WriteLine(AppConstants.InvalidSpeciesMessage);
                input = Console.ReadLine()?.ToLower() ?? "";
            }
            return input;
        }

        private static string GetValidAge(string id)
        {
            Console.WriteLine(string.Format(AppConstants.AgePromptFormat, id));
            return Console.ReadLine() ?? UnknownAge;
        }

        private static string GetPhysicalDescription(string id)
        {
            Console.WriteLine(string.Format(AppConstants.PhysicalDescriptionPromptFormat, id));
            return Console.ReadLine() ?? DefaultValue;
        }

        private static string GetPersonalityDescription(string id)
        {
            Console.WriteLine(string.Format(AppConstants.PersonalityDescriptionPromptFormat, id));
            return Console.ReadLine() ?? DefaultValue;
        }

        private static string GetNickname(string id)
        {
            Console.WriteLine(string.Format(AppConstants.NicknamePromptFormat, id));
            return Console.ReadLine() ?? DefaultValue;
        }

        public void EnsureAgesAndDescriptionsComplete()
        {
            var animals = ListAll();

            if (animals.Count == 0)
            {
                Console.WriteLine(AppConstants.NoAnimalsFoundMessage);
            }

            bool hasChanges = false;
            foreach (var animal in animals)
            {
                if (string.IsNullOrEmpty(animal.Age) || animal.Age == UnknownAge)
                {
                    Console.WriteLine(string.Format(AppConstants.AgePromptComplete, animal.Id, animal.Species));
                    string newAge = GetValidAge(animal.Id);
                    animal.SetAge(newAge);
                    hasChanges = true;
                }
                if (string.IsNullOrEmpty(animal.PhysicalDescription) || animal.PhysicalDescription == DefaultValue)
                {
                    Console.WriteLine(string.Format(AppConstants.PhysicalDescriptionPromptComplete, animal.Id, animal.Species));
                    string newDescription = GetPhysicalDescription(animal.Id);
                    animal.SetPhysicalDescription(newDescription);
                    hasChanges = true;
                }
            }
            if (hasChanges)
            {
                repository.SaveChanges();
            }
            Console.WriteLine(AppConstants.AgeAndDescriptionCompleteMessage);
        }

        public void EnsureNicknamesAndPersonalityComplete()
        {
            var animals = ListAll();

            if (animals.Count == 0)
            {
                Console.WriteLine(AppConstants.NoAnimalsFoundMessage);
            }

            bool hasChanges = false;
            foreach (var animal in animals.Where(a => !string.IsNullOrEmpty(a.Id)))
            {
                if (string.IsNullOrEmpty(animal.Nickname) || animal.Nickname == DefaultValue)
                {
                    string newNickname = GetNickname(animal.Id);
                    animal.SetNickname(newNickname);
                    hasChanges = true;
                }

                if (string.IsNullOrEmpty(animal.PersonalityDescription) || animal.PersonalityDescription == DefaultValue)
                {
                    string newPersonality = GetPersonalityDescription(animal.Id);
                    animal.SetPersonalityDescription(newPersonality);
                    hasChanges = true;
                        
                }
            }
            if (hasChanges)
            {
                repository.SaveChanges();
            }
            Console.WriteLine(AppConstants.NicknamePersonalityCompleteMessage);
        }

        public void EditAnimalAge()
        {
            Console.WriteLine(string.Format(AppConstants.EnterAnimalIdPrompt, "age"));
            string? id = Console.ReadLine();
            var animals = ListAll();
            var animal = animals.FirstOrDefault(a => a.Id == id);
            if (animal != null)
            {
                Console.WriteLine(string.Format(AppConstants.CurrentAgeFormat, id, animal.Age));
                string newAge = GetValidAge(animal.Id);
                animal.SetAge(newAge);
                repository.SaveChanges();
                Console.WriteLine(string.Format(AppConstants.UpdatedAgeFormat, id, newAge));
            }
            else
            {
                Console.WriteLine(AppConstants.AnimalNotFoundMessage);
            }
        }

        public void EditAnimalPersonality()
        {
            Console.WriteLine(string.Format(AppConstants.EnterAnimalIdPrompt, "personality"));
            string? id = Console.ReadLine();
            var animals = ListAll();
            var animal = animals.FirstOrDefault(a => a.Id == id);
            if (animal != null)
            {
                Console.WriteLine(string.Format(AppConstants.CurrentPersonalityFormat, id, animal.PersonalityDescription));
                string newPersonality = GetPersonalityDescription(animal.Id);
                animal.SetPersonalityDescription(newPersonality);
                repository.SaveChanges();
                Console.WriteLine(AppConstants.UpdatedPersonalityMessage);
            }
            else
            {
                Console.WriteLine(AppConstants.AnimalNotFoundMessage);
            }
        }

        public void DisplayAnimalsWithCharacteristic(string species)
        {
            Console.WriteLine(string.Format(AppConstants.CharacteristicSearchPromptFormat, species));            
            string? characteristic = Console.ReadLine();

            if (!string.IsNullOrEmpty(characteristic))
            {
                var animals = ListAll()
                    .Where(a => a.Species.Equals(species, StringComparison.OrdinalIgnoreCase) &&
                        !string.IsNullOrEmpty(a.Id) &&
                        (a.PhysicalDescription.Contains(characteristic, StringComparison.OrdinalIgnoreCase) ||
                         a.PersonalityDescription.Contains(characteristic, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                if (animals.Count != 0)
                {
                    Console.WriteLine(string.Format(AppConstants.CharacteristicResultsFormat, species, characteristic));
                    foreach (var animal in animals)
                    {
                        Console.WriteLine();
                        animal.DisplayInfo();
                    }
                }
                else
                {
                    Console.WriteLine(string.Format(AppConstants.NoCharacteristicMatchFormat, species, characteristic));
                }
            }
        }
              
        public void DisplayCatsWithCharacteristic() => DisplayAnimalsWithCharacteristic("cat");
        public void DisplayDogsWithCharacteristic() => DisplayAnimalsWithCharacteristic("dog");

        private static string GetYesNoInput()
        {
            string input = Console.ReadLine()?.ToLower() ?? CanNo;
            while (input != CanYes && input != CanNo)
            {
                Console.WriteLine(AppConstants.InvalidYesNoMessage);
                input = Console.ReadLine()?.ToLower() ?? CanNo;
            }
            return input;
        }
    }
}
