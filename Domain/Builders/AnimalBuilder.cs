using ContosoPets.Domain.Entities;
using ContosoPets.Domain.Constants;

namespace ContosoPets.Domain.Builders
{
    public class AnimalBuilder
    {
        private string _species = string.Empty;
        private string _id = string.Empty;
        private string _age = "?";
        private string _physicalDescription = "tbd";
        private string _personalityDescription = "tbd";
        private string _nickname = "tbd";

        private AnimalBuilder()
        {
            Reset();
        }

        public static AnimalBuilder Builder()
        {
          return new AnimalBuilder();
        }

        private void Reset()
        {
            _species = string.Empty;
            _id = string.Empty;
            _age = "?";
            _physicalDescription = "tbd";
            _personalityDescription = "tbd";
            _nickname = "tbd";
        }

        public AnimalBuilder WithSpecies(string species)
        {
            _species = species.ToLower();
            return this;
        }

        public AnimalBuilder WithId(string id)
        {
            _id = id;
            return this;
        }

        public AnimalBuilder WithAge(string age)
        {
            _age = age;
            return this;
        }

        public AnimalBuilder WithPhysicalDescription(string physicalDescription)
        {
            _physicalDescription = physicalDescription;
            return this;
        }

        public AnimalBuilder WithPersonalityDescription(string personalityDescription)
        {
            _personalityDescription = personalityDescription;
            return this;
        }

        public AnimalBuilder WithNickname(string nickname)
        {
            _nickname = nickname;
            return this;
        }

        public Animal Build()
        {             
            if (string.IsNullOrEmpty(_species))
                throw new InvalidOperationException("Species must be specified.");
            if (string.IsNullOrEmpty(_id))
                throw new InvalidOperationException("ID must be specified."); // ID automatically generated donc pas indispensable

            Animal result = _species switch
            {
                "dog" => new Dog("dog",_id, _age, _physicalDescription, _personalityDescription, _nickname),
                "cat" => new Cat("cat",_id, _age, _physicalDescription, _personalityDescription, _nickname),
                _ => throw new InvalidOperationException(AppConstants.InvalidSpeciesMessage)
            };

            Reset();
            return result;
        }
    }
}
