using ContosoPets.Domain.Constants;

namespace ContosoPets.Application.UseCases.Animals
{
    public class AddAnimalRequest
    {
        public required string Species { get; set; }
        public string Age { get; set; } = AppConstants.UnknownAge;
        public string PhysicalDescription { get; set; } = AppConstants.DefaultValue;
        public string PersonalityDescription { get; set; } = AppConstants.DefaultValue;
        public string Nickname { get; set; } = AppConstants.DefaultValue;
    }
}
