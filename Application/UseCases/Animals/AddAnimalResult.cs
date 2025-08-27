using ContosoPets.Domain.Entities;

namespace ContosoPets.Application.UseCases.Animals
{
    public class AddAnimalResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public Animal? Animal { get; set; }
    }
}
