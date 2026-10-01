using MiniCrm.Domain.Validation;

namespace MiniCrm.Domain.Entities;

public sealed class Customer
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public required string Name { get; init; }
    public required string Email { get; init; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public bool IsActive { get; init; } = true;

    private readonly List<Contact> _contacts = [];
    public IReadOnlyList<Contact> Contacts => _contacts;

    public IReadOnlyList<ValidationError> AddContacts(List<Contact> contacts)
    {
        var validationErrors = new List<ValidationError>();

        foreach (var contact in contacts)
        {
            var errors = AddContact(contact);
            validationErrors.AddRange(errors);
        }

        return validationErrors;
    }

    public IReadOnlyList<ValidationError> AddContact(Contact contact)
    {
        if (contact.CustomerId != Id)
        {
            return [new ValidationError(nameof(Contact.CustomerId), "Contact's CustomerId does not match the Customer's Id.")];
        }

        ContactValidator contactValidator = new ContactValidator();
        var validationErrors = contactValidator.Validate(contact);
        if (validationErrors.Any())
        {
            return validationErrors;
        }

        _contacts.Add(contact);
        return [];
    }
}
