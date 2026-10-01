using MiniCrm.Domain.Entities;
using MiniCrm.Domain.Repository;

namespace MiniCrm.Domain.Tests.Repository;

public class CustomerRepositoryTest
{
    [Fact]
    public async Task AddCustomer_ThenGetById_ReturnsCustomer()
    {
        var repository = new InMemoryCustomerRepository();
        var customer = new Customer { Id = Guid.NewGuid(), Name = "Test Customer", Email = "test@example.com" };

        await repository.AddAsync(customer, CancellationToken.None);

        Assert.Same(customer, await repository.GetByIdAsync(customer.Id, CancellationToken.None));
    }

    [Fact]
    public async Task GetById_WithNonExistingId_ReturnsNull()
    {
        var repository = new InMemoryCustomerRepository();

        Assert.Null(await repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task ModifyingListReturnedByGetAll_DoesNotAffectRepositoryStore()
    {
        var repository = new InMemoryCustomerRepository();
        var customer = new Customer { Id = Guid.NewGuid(), Name = "Test Customer", Email = "test@example.com" };
        await repository.AddAsync(customer, CancellationToken.None);

        var customers = await repository.GetAllAsync(CancellationToken.None);

        Assert.Throws<InvalidCastException>(() => ((List<Customer>)customers).Clear());
    }

    [Fact]
    public async Task UpdateExistingCustomer_ReplacesCustomer()
    {
        var repository = new InMemoryCustomerRepository();
        var customer = new Customer { Id = Guid.NewGuid(), Name = "Test Customer", Email = "test@example.com" };
        var updatedCustomer = new Customer { Id = customer.Id, Name = "Updated Customer", Email = "updated@example.com" };
        await repository.AddAsync(customer, CancellationToken.None);

        Assert.True(await repository.UpdateAsync(updatedCustomer, CancellationToken.None));
        Assert.Same(updatedCustomer, await repository.GetByIdAsync(customer.Id, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateNonExistingCustomer_ReturnsFalse()
    {
        var repository = new InMemoryCustomerRepository();
        var customer = new Customer { Id = Guid.NewGuid(), Name = "Test Customer", Email = "test@example.com" };

        Assert.False(await repository.UpdateAsync(customer, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteExistingCustomer_RemovesCustomer()
    {
        var repository = new InMemoryCustomerRepository();
        var customer = new Customer { Id = Guid.NewGuid(), Name = "Test Customer", Email = "test@example.com" };
        await repository.AddAsync(customer, CancellationToken.None);

        Assert.True(await repository.DeleteAsync(customer.Id, CancellationToken.None));
        Assert.Null(await repository.GetByIdAsync(customer.Id, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteNonExistingCustomer_ReturnsFalse()
    {
        var repository = new InMemoryCustomerRepository();

        Assert.False(await repository.DeleteAsync(Guid.NewGuid(), CancellationToken.None));
    }


}