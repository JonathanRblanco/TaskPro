using TaskPro.Domain.Entities.Base;
using TaskPro.Domain.Enums;
using TaskPro.Domain.Exceptions;
using TaskPro.Domain.ValueObjects;

namespace TaskPro.Domain.Entities
{
    public class User : BaseEntity
    {
        private User() { }
        public FullName Name { get; private set; } = null!;
        public Email Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public ApplicationRole Role { get; private set; }
        private readonly List<ProjectMember> _memberships = [];
        public IReadOnlyCollection<ProjectMember> Memberships => _memberships.AsReadOnly();
        public static User Create(string firstName, string lastName,
                                  string email, string passwordHash, ApplicationRole role = ApplicationRole.Member)
        {
            return new User
            {
                Name = FullName.Create(firstName, lastName),
                Email = Email.Create(email),
                PasswordHash = passwordHash,
                Role = role
            };
        }

        public void UpdateProfile(string firstName, string lastName)
        {
            Name = FullName.Create(firstName, lastName);
            MarkAsUpdated();
        }

        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new DomainException("Password hash cannot be empty.");

            PasswordHash = newPasswordHash;
            MarkAsUpdated();
        }
        public void ChangeApplicationRole(ApplicationRole newRole)
        {
            Role = newRole;
            MarkAsUpdated();
        }
    }
}
