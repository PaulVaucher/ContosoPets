namespace ContosoPets.Domain.ValueObjects
{
    public sealed class AnimalId : IEquatable<AnimalId>
    {
        public string Value { get; }

        public AnimalId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("AnimalId cannot be null or empty.", nameof(value));

            Value = value;
        }

        public override bool Equals(object? obj) => Equals(obj as AnimalId);

        public bool Equals(AnimalId? other) => other != null && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;

        public static implicit operator string(AnimalId id) => id.Value;
        public static explicit operator AnimalId(string value) => new AnimalId(value);
    }
}