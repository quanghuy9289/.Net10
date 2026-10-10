using MiniCrm.Domain.Abstractions;
using MiniCrm.Domain.Common;
using MiniCrm.Domain.Entities;
using MiniCrm.Domain.Validation;

public sealed class NameRequiredValidator : ICustomerValidator
{
    public Result<IReadOnlyList<ValidationError>> ValidateCustomer(CustomerRequest request)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors.Add(new ValidationError("Name", "Customer name is required."));
        }
        else if (request.Name.Length > 100)
        {
            errors.Add(new ValidationError("Name", "Customer name cannot exceed 100 characters."));
        }

        return errors.Count > 0
            ? Result<IReadOnlyList<ValidationError>>.Failure(errors)
            : Result<IReadOnlyList<ValidationError>>.Success([]);
    }
}