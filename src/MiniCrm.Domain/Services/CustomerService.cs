using MiniCrm.Domain.Entities;
using MiniCrm.Domain.Common;
using MiniCrm.Domain.Validation;
using MiniCrm.Domain.Abstractions;
using Microsoft.Extensions.Logging;

namespace MiniCrm.Domain.Services;

public sealed class CustomerService(ICustomerRepository customerRepository, IEnumerable<ICustomerValidator> customerValidators, ILogger<CustomerService> logger) : ICustomerService
{
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly IEnumerable<ICustomerValidator> _customerValidators = customerValidators;
    private readonly ILogger<CustomerService> _logger = logger;

    public async Task<Result<Customer>> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        return customer is null
            ? Result<Customer>.Failure([new ValidationError(nameof(Customer.Id), "Customer not found.")])
            : Result<Customer>.Success(customer);
    }

    public async Task<Result<Customer>> AddCustomerAsync(CustomerRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var validationErrors = new List<ValidationError>();

        foreach (var validator in _customerValidators)
        {
            var result = validator.ValidateCustomer(request);
            if (!result.IsSuccess)
            {
                validationErrors.AddRange(result.Errors);
            }
        }
        if (validationErrors.Any())
        {
            return Result<Customer>.Failure(validationErrors);
        }

        var customer = new Customer
        {
            Name = request.Name,
            Email = request.Email,
        };

        var isAdded = await _customerRepository.AddAsync(customer, cancellationToken);
        if (!isAdded)
        {
            return Result<Customer>.Failure([new ValidationError(nameof(Customer.Id), "Customer already exists.")]);
        }

        return Result<Customer>.Success(customer);
    }

    public async Task<Result<Customer>> UpdateCustomerAsync(CustomerRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var validationErrors = new List<ValidationError>();

        foreach (var validator in _customerValidators)
        {
            var result = validator.ValidateCustomer(request);
            if (!result.IsSuccess)
            {
                validationErrors.AddRange(result.Errors);
            }
        }
        if (validationErrors.Any())
        {
            return Result<Customer>.Failure(validationErrors);
        }

        var existingCustomer = await GetCustomerByIdAsync(request.Id, cancellationToken);
        if (!existingCustomer.IsSuccess || existingCustomer.Value is null)
        {
            return Result<Customer>.Failure([new ValidationError(nameof(Customer.Id), "Customer not found.")]);
        }

        var customer = new Customer
        {
            Id = existingCustomer.Value.Id,
            Name = request.Name,
            Email = request.Email,
            CreatedAt = existingCustomer.Value.CreatedAt,
            IsActive = existingCustomer.Value.IsActive,
        };

        customer.AddContacts([.. existingCustomer.Value.Contacts]);

        var isUpdated = await _customerRepository.UpdateAsync(customer, cancellationToken);
        if (!isUpdated)
        {
            return Result<Customer>.Failure([new ValidationError(nameof(Customer.Id), "Customer not found.")]);
        }

        return Result<Customer>.Success(customer);
    }

    public async Task<Result<bool>> DeleteCustomerAsync(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Customer ID cannot be empty.", nameof(id));
        }

        bool isDeleted = await _customerRepository.DeleteAsync(id, cancellationToken);
        if (!isDeleted)
        {
            return Result<bool>.Failure([new ValidationError(nameof(Customer.Id), "Customer not found.")]);
        }

        return Result<bool>.Success(true);
    }

    public async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _customerRepository.GetAllAsync(cancellationToken);
    }

    public Task<int> CountCustomerAsync(CancellationToken cancellationToken)
    {
        return _customerRepository.CountAsync(cancellationToken);
    }
}

