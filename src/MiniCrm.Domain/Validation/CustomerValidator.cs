using System.ComponentModel.DataAnnotations;
using MiniCrm.Domain.Abstractions;
using MiniCrm.Domain.Common;
using MiniCrm.Domain.Entities;

namespace MiniCrm.Domain.Validation;

public class CustomerRequest
{
    public Guid Id { get; init; }

    [Required(ErrorMessage = "Customer name is required.")]
    [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters.")]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "Customer email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
    public string Email { get; init; } = string.Empty;
}

// public class CustomerValidator : ICustomerValidator
// {
//     public Result<IReadOnlyList<ValidationError>> ValidateCustomer(CustomerRequest request)
//     {
//         ArgumentNullException.ThrowIfNull(request);

//         var validationResults = new List<ValidationResult>();
//         Validator.TryValidateObject(
//             request,
//             new ValidationContext(request),
//             validationResults,
//             validateAllProperties: true);

//         var validationErrors = validationResults
//             .SelectMany(result => result.MemberNames.DefaultIfEmpty(string.Empty)
//                 .Select(member => new ValidationError(
//                     member,
//                     result.ErrorMessage ?? (string.IsNullOrEmpty(member)
//                         ? "The customer request is invalid."
//                         : "The value is invalid."))))
//             .ToList();

//         if (validationErrors.Count > 0)
//         {
//             // Additional custom validation logic can be added here if needed
//             return Result<IReadOnlyList<ValidationError>>.Failure(validationErrors);
//         }

//         return Result<IReadOnlyList<ValidationError>>.Success([]);
//     }

//     public string ClassifyCustomer(Customer customer) => customer switch
//     {
//         { IsActive: false } => "Inactive",
//         { Contacts.Count: 0 } => "No contacts",
//         { Contacts: var contacts } when !contacts.Any(contact => contact.Type == ContactType.Email) =>
//             "Missing email",
//         _ => "Complete"
//     };
// }
