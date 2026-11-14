using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MiniCrm.Domain.Abstractions;
using MiniCrm.Domain.Repository;

public sealed class MiniCrmApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing"); // Set the environment to "Testing" for integration tests

        builder.ConfigureTestServices(services =>
        {
            // Additional service configuration for testing can be done here
            services.RemoveAll<ICustomerRepository>();
            services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>(); // Use an in-memory repository for testing
        });
    }
}

