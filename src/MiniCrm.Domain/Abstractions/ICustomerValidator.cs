using MiniCrm.Domain.Common;
using MiniCrm.Domain.Entities;
using MiniCrm.Domain.Validation;

namespace MiniCrm.Domain.Abstractions;

public interface ICustomerValidator
{
    Result<IReadOnlyList<ValidationError>> ValidateCustomer(CustomerRequest request);
}

