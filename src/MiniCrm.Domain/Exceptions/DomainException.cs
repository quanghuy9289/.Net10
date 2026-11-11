namespace MiniCrm.Domain.Exceptions;

public abstract class DomainException(string message) : Exception(message);

public sealed class CustomerNotFoundException(Guid customerId) : DomainException($"Customer with ID '{customerId}' was not found.")
{
    public Guid CustomerId { get; } = customerId;
}

public sealed class DuplicateEmailException(string email) : DomainException($"Email '{email}' is already used by another customer.")
{
    public string Email { get; } = email;
}