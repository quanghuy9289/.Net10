using MiniCrm.Domain.Entities;

namespace MiniCrm.Domain.Abstractions;

public interface ICustomerRepository
{
    public Guid InstanceId { get; }

    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> AddAsync(Customer customer, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Customer customer, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken);
    Task<int> CountAsync(CancellationToken cancellationToken);
}