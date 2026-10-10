using MiniCrm.Domain.Common;
using MiniCrm.Domain.Entities;
using MiniCrm.Domain.Validation;

namespace MiniCrm.Domain.Abstractions;

public interface ICustomerService
{
    Task<Result<Customer>> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<Customer>> AddCustomerAsync(CustomerRequest request, CancellationToken cancellationToken);
    Task<Result<Customer>> UpdateCustomerAsync(CustomerRequest request, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteCustomerAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken);
    Task<int> CountCustomerAsync(CancellationToken cancellationToken);
}