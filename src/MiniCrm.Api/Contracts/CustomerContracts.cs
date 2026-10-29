using System.ComponentModel.DataAnnotations;
using MiniCrm.Domain.Entities;

namespace MiniCrm.Api.Contracts;

public sealed record CreateCustomerRequest
{
    [Required, StringLength(200)]
    public string Name {get; init;} = "";

    [Required, EmailAddress]
    public string Email {get; init;} = "";
}

public sealed record UpdateCustomerRequest
{
    [Required, StringLength(200)]
    public string Name {get; init;} = "";

    [Required, EmailAddress]
    public string Email {get; init;} = "";

    public bool IsActive {get; init;}
}

public sealed record CustomerResponse(Guid Id, string Name, string Email, DateTimeOffset CreatedAt, bool IsActive)
{
    public static CustomerResponse From(Customer c) => new (c.Id, c.Name, c.Email, c.CreatedAt, c.IsActive);
}

public sealed record ContactResponse(Guid Id, Guid CustomerId, ContactType Type, string Value, Note Note)
{
    public static ContactResponse From(Contact c) => new (c.Id, c.CustomerId, c.Type, c.Value, c.Note);
}