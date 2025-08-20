using ContosoPets.Domain.Entities;
using ContosoPets.Infrastructure.AssemblyReferences;

namespace ContosoPets.Application.UseCases.Animals
{
    public interface IAnimalService : IAssemblyReference
    {
        List<Animal> ListAll();
        void AddNewAnimal();
        void EnsureAgesAndDescriptionsComplete();
        void EnsureNicknamesAndPersonalityComplete();
        void EditAnimalAge();
        void EditAnimalPersonality();
        void DisplayCatsWithCharacteristic();
        void DisplayDogsWithCharacteristic();

    }
}
