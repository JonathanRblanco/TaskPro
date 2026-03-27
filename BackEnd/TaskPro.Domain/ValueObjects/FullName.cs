using TaskPro.Domain.Exceptions;

namespace TaskPro.Domain.ValueObjects
{
    public sealed class FullName
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string DisplayName => $"{FirstName} {LastName}".Trim();

        private FullName(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public static FullName Create(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new DomainException("First name is required.");
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new DomainException("Last name is required.");
            }

            return new FullName(firstName.Trim(), lastName.Trim());
        }

        public override bool Equals(object? obj) =>
            obj is FullName other &&
            FirstName == other.FirstName &&
            LastName == other.LastName;

        public override int GetHashCode() => HashCode.Combine(FirstName, LastName);
        public override string ToString() => DisplayName;
    }
}
