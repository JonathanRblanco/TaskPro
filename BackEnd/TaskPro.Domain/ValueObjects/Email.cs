using TaskPro.Domain.Exceptions;

namespace TaskPro.Domain.ValueObjects
{
    public sealed class Email
    {
        public string Value { get; }
        private Email(string value) => Value = value;
        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new DomainException("Email cannot be empty.");
            }

            var normalized = value.Trim().ToLowerInvariant();

            if (!normalized.Contains('@') || normalized.Length < 5)
            {
                throw new DomainException($"'{value}' is not a valid email address.");
            }

            return new Email(normalized);
        }
        public override bool Equals(object? obj) =>
            obj is Email other && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;
    }
}
