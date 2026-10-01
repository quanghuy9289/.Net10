using System.Net.Mail;
using MiniCrm.Domain.Entities;

namespace MiniCrm.Domain.Validation;

public sealed class ContactValidator
{
    public IReadOnlyList<ValidationError> Validate(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);

        return contact.Type switch
        {
            ContactType.Unknown => [new ValidationError("Unknown", "Contact type must be specified.")],
            ContactType.Email => IsValidEmail(contact.Value)
                ? []
                : [new ValidationError("Email", "Contact value must be a valid email address.")],
            ContactType.Phone => IsValidPhoneNumber(contact.Value)
                ? []
                : [new ValidationError("Phone", "Contact value must be a valid phone number.")],
            ContactType.Address => IsValidAddress(contact.Value)
                ? []
                : [new ValidationError("Address", "Contact value must be a valid address.")],
            _ => [new ValidationError("Other", "Contact type is not supported.")]
        };
    }

    private static bool IsValidEmail(string? value) =>
        !string.IsNullOrWhiteSpace(value) && MailAddress.TryCreate(value, out _);

    private static bool IsValidPhoneNumber(string? value) =>
        !string.IsNullOrWhiteSpace(value) && value.All(char.IsAsciiDigit) && value.Length is >= 10 and <= 15;

    private static bool IsValidAddress(string? value) =>
        !string.IsNullOrWhiteSpace(value) && value.Length is >= 5 and <= 200;
}
