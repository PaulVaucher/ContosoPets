namespace ContosoPets.Infrastructure.Entities
{
    public class NHAnimal
    {
        public virtual string Id { get; protected set; } = string.Empty;
        public virtual string Species { get; protected set; } = string.Empty;
        public virtual string Age { get; protected set; } = "?";
        public virtual string PhysicalDescription { get; protected set; } = string.Empty;
        public virtual string PersonalityDescription { get; protected set; } = string.Empty;
        public virtual string Nickname { get; protected set; } = string.Empty;
    }
}
