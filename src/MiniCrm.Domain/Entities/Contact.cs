namespace MiniCrm.Domain.Entities;

public sealed class Contact
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public ContactType Type { get; init; } = ContactType.Unknown;
    public string Value { get; init; } = string.Empty;
    public Note Note { get; init; } = new Note();
}