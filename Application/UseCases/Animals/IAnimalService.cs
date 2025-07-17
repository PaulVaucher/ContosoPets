using ContosoPets.Domain.Entities;

namespace ContosoPets.Application.UseCases.Animals
{
    public interface IAnimalService
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
