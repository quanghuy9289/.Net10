using MiniCrm.Domain.Abstractions;
using MiniCrm.Domain.Entities;
using MiniCrm.Domain.Repository;
using MiniCrm.Domain.Services;
using MiniCrm.Domain.Validation;

namespace MiniCrm.Domain.Tests.Services;

public sealed class CustomerServiceTest
{

    [Fact]
    public async Task GetAllAsync_returns_customers_from_repository()
    {
        var customers = new[]
        {
            new Customer { Name = "Ada Lovelace", Email = "ada@example.com" },
            new Customer { Name = "Grace Hopper", Email = "grace@example.com" }
        };
        ICustomerRepository repository = new InMemoryCustomerRepository();
        await repository.AddAsync(customers[0], CancellationToken.None);
        await repository.AddAsync(customers[1], CancellationToken.None);
        var validator = new CustomerValidator();

        var service = new CustomerService(repository, validator);

        var result = await service.GetAllAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByIdAsync_returns_customer_from_repository()
    {
        var customer = new Customer { Id = Guid.NewGuid(), Name = "Linus Torvalds", Email = "linus@example.com" };
        var repository = new InMemoryCustomerRepository();
        await repository.AddAsync(customer, CancellationToken.None);

        var validator = new CustomerValidator();

        var service = new CustomerService(repository, validator);

        var result = await service.GetCustomerByIdAsync(customer.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.Equal(customer.Id, result.Value.Id);
    }

    [Fact]
    public async Task CreateAsync_persists_customer_success()
    {
        var customer = new Customer { Name = "Katherine Johnson", Email = "katherine@example.com" };
        var repository = new InMemoryCustomerRepository();
        var validator = new CustomerValidator();

        var service = new CustomerService(repository, validator);

        var result = await service.AddCustomerAsync(new CustomerRequest
        {
            Name = customer.Name,
            Email = customer.Email
        }, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task UpdateAsync_persists_and_returns_customer()
    {
        var customer = new Customer { Id = Guid.NewGuid(), Name = "Updated Customer", Email = "updated@example.com" };
        var repository = new InMemoryCustomerRepository();
        await repository.AddAsync(customer, CancellationToken.None);
        var validator = new CustomerValidator();

        var service = new CustomerService(repository, validator);

        var result = await service.UpdateCustomerAsync(new CustomerRequest
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email
        }, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteAsync_delegates_to_repository()
    {
        var id = Guid.NewGuid();
        var repository = new InMemoryCustomerRepository();
        await repository.AddAsync(new Customer { Id = id, Name = "Customer to Delete", Email = "delete@example.com" }, CancellationToken.None);
        var validator = new CustomerValidator();

        var service = new CustomerService(repository, validator);

        var result = await service.DeleteCustomerAsync(id, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}
