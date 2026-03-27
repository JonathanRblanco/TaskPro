namespace TaskPro.Domain.Exceptions
{
    public sealed class NotFoundException : DomainException
    {
        public NotFoundException(string entityName, Guid id) : base($"{entityName} with id: '{id}' was not found.") { }

        public NotFoundException(string message) : base(message) { }
    }
}
