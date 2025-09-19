namespace ContosoPets.Domain.ValueObjects
{
    public sealed class AnimalId : IEquatable<AnimalId>
    {
        public string Value { get; }
        public AnimalId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Animal ID cannot be null or empty.", nameof(value));
            }
            Value = value;
        }
        public override bool Equals(object? obj)
        {
            return Equals(obj as AnimalId);
        }
        public bool Equals(AnimalId? other)
        {
            return other != null && Value == other.Value;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }        
        public override string ToString()
        {
            return Value;
        }
        public static implicit operator string(AnimalId id)
        {
            return id.Value;
        }
        public static explicit operator AnimalId(string value)
        {
            return new AnimalId(value);
        }
    }
}
