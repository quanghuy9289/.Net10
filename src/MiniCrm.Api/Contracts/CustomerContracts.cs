using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MiniCrm.Api.Helper;
using MiniCrm.Domain.Entities;

namespace MiniCrm.Api.Contracts;

public sealed record CreateCustomerRequest
{
    [Required, StringLength(200)]
    public string Name {get; init;} = string.Empty;

    [Required, EmailAddress]
    public string Email {get; init;} = string.Empty;

    [JsonConverter(typeof(DayMonthYearDateOnlyConverter))]
    public DateOnly? FirstContactedAt {get; init;} = null;
}


public sealed record UpdateCustomerRequest
{
    [Required, StringLength(200)]
    public string Name {get; init;} = string.Empty;

    [Required, EmailAddress]
    public string Email {get; init;} = string.Empty;

    [JsonConverter(typeof(DayMonthYearDateOnlyConverter))]
    public DateOnly? FirstContactedAt {get; init;} = null;

    public bool IsActive {get; init;}
}

public sealed record CustomerResponse
{
    public required Guid Id {get; init;} = Guid.CreateVersion7();
    public required string Name {get; init;} = string.Empty;
    public required string Email {get; init;} = string.Empty;
    public required bool IsActive {get; init;}
    public required DateTimeOffset CreatedAt {get; init;} = DateTimeOffset.UtcNow;
    
    [JsonConverter(typeof(DayMonthYearDateOnlyConverter))]
    public DateOnly? FirstContactedAt {get; init;} = null;
}

public static class CustomerMappings
{
    public static CustomerResponse ToResponse(this Customer c) => new ()
    {
        Id = c.Id,
        Name = c.Name,
        Email = c.Email,
        IsActive = c.IsActive,
        CreatedAt = c.CreatedAt,
        FirstContactedAt = c.FirstContactedAt,
    };

    public static Customer ToDomain(this CreateCustomerRequest request) => new ()
    {
        Name = request.Name,
        Email = request.Email,
        FirstContactedAt = request.FirstContactedAt,
    };

    public static Customer ToDomain(this UpdateCustomerRequest request) => new ()
    {
        Name = request.Name,
        Email = request.Email,
        FirstContactedAt = request.FirstContactedAt,
        IsActive = request.IsActive,
    };
}

public sealed record ContactResponse(Guid Id, Guid CustomerId, ContactType Type, string Value, Note Note);

public static class ContactMappings
{
    public static ContactResponse ToResponse(this Contact c) => new (c.Id, c.CustomerId, c.Type, c.Value, c.Note);
}
