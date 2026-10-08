using MiniCrm.Domain.Abstractions;
using MiniCrm.Domain.Entities;

public sealed class SeedCustomer : IHostedService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<SeedCustomer> _logger;

    public SeedCustomer(ICustomerRepository customerRepository, ILogger<SeedCustomer> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

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

        _logger.LogInformation("Customers seeded successfully.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
