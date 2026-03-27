namespace TaskPro.Domain.Exceptions
{
    public sealed class UnauthorizedDomainException(string message) : DomainException(message)
    {
    }
}
