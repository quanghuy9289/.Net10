using System.Collections.Concurrent;
using MiniCrm.Domain.Abstractions;
using MiniCrm.Domain.Entities;
using MiniCrm.Domain.Validation;

namespace MiniCrm.Domain.Repository;

public sealed class InMemoryCustomerRepository : ICustomerRepository
{
    private readonly ConcurrentDictionary<Guid, Customer> _customers = [];

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_customers.TryGetValue(id, out var customer) ? customer : null);
    }

    public Task<bool> AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        return Task.FromResult(_customers.TryAdd(customer.Id, customer));
    }

    public Task<bool> UpdateAsync(Customer customer, CancellationToken cancellationToken)
    {
        if (!_customers.TryGetValue(customer.Id, out var existing))
        {
            return Task.FromResult(false);
        }
        return Task.FromResult(_customers.TryUpdate(customer.Id, customer, existing));
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_customers.TryRemove(id, out _));
    }

    public Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyList<Customer>>([.. _customers.Values]);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return Task.Delay(3000, cancellationToken).ContinueWith(_ => _customers.Count);
    }
}