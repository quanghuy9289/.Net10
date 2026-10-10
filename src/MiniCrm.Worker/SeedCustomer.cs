using MiniCrm.Domain.Abstractions;
using MiniCrm.Domain.Entities;

public sealed class SeedCustomer(IServiceScopeFactory serviceScopeFactory, ILogger<SeedCustomer> logger) : IHostedService
{
    private readonly ICustomerRepository _customerRepository = serviceScopeFactory.CreateAsyncScope().ServiceProvider.GetRequiredService<ICustomerRepository>();
    private readonly ILogger<SeedCustomer> _logger = logger;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var customers = new List<Customer>
        {
            new Customer { Id = Guid.NewGuid(), Name = "John Doe", Email = "test@example.com" },
            new Customer { Id = Guid.NewGuid(), Name = "Jane Smith", Email = "smith@gmail.com" },
            new Customer { Id = Guid.NewGuid(), Name = "Alice Johnson", Email = "john-1@gmail.com" }
        };

        _logger.LogInformation("Seeding customers..."); // log that we are seeding customers

        foreach (var customer in customers)
        {
            await _customerRepository.AddAsync(customer, cancellationToken);
        }

        var count = await _customerRepository.CountAsync(cancellationToken);
        _logger.LogInformation($"Seeded {count} customers."); // log the number of customers

        _logger.LogInformation("Customers seeded successfully.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
