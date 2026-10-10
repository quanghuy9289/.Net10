using MiniCrm.Domain.Abstractions;
using MiniCrm.Domain.Common;
using MiniCrm.Domain.Entities;
using MiniCrm.Domain.Validation;

public sealed class EmailFormatValidator : ICustomerValidator
{
    public Result<IReadOnlyList<ValidationError>> ValidateCustomer(CustomerRequest request)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            errors.Add(new ValidationError("Email", "Customer email is required."));
        }
        else if (request.Email.Length > 255)
        {
            errors.Add(new ValidationError("Email", "Customer email cannot exceed 255 characters."));
        }
        else if (!IsValidEmail(request.Email))
        {
            errors.Add(new ValidationError("Email", "Invalid email format."));
        }

        return errors.Count > 0
            ? Result<IReadOnlyList<ValidationError>>.Failure(errors)
            : Result<IReadOnlyList<ValidationError>>.Success([]);
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}