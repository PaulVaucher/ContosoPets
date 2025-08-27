namespace ContosoPets.Application.UseCases.Animals
{
    public class AddAnimalRequest
    {
        public required string Species { get; set; }
        public required string Age { get; set; }
        public required string PhysicalDescription { get; set; }
        public required string PersonalityDescription { get; set; }
        public required string Nickname { get; set; }
    }
}
