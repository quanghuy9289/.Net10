using MiniCrm.Domain.Abstractions;
using MiniCrm.Domain.Repository;
using MiniCrm.Domain.Services;
using MiniCrm.Domain.Validation;
using MiniCrm.Worker;

var builder = Host.CreateApplicationBuilder(args); // config + logging + DI

builder.Services.AddSingleton<InMemoryStore>(); // register InMemoryStore as a Singleton
builder.Services.AddScoped<ICustomerValidator, NameRequiredValidator>(); // register ICustomerValidator with its implementation
builder.Services.AddScoped<ICustomerValidator, EmailFormatValidator>(); // register ICustomerValidator with its implementation
builder.Services.AddScoped<ICustomerRepository, InMemoryCustomerRepository>(); // register ICustomerRepository with its implementation
builder.Services.AddScoped<ICustomerService, CustomerService>(); // register ICustomerService with its implementation


//builder.Services.AddScoped<IServiceScopeFactory, IServiceScopeFactory>(); // register IServiceScopeFactory with its implementation
//builder.Services.AddHostedService<LifecycleDemo>(); // register LifecycleDemo as a Singleton IHostedService

builder.Services.AddHostedService<SeedCustomer>(); // register SeedCustomer as a Singleton IHostedService
builder.Services.AddHostedService<Worker>(); // register Worker as a Singleton IHostedService

// builder.Services.Configure<HostOptions>(hostOptions =>
// {
//     hostOptions.ShutdownTimeout = TimeSpan.FromSeconds(10); // set the shutdown timeout to 10 seconds
// });


var host = builder.Build(); // lock container and create the host

for (int i = 1; i <= 2; i++)
{
    using var scope = host.Services.CreateScope(); // create a new scope for each iteration
    var sp = scope.ServiceProvider; // get the service provider for the current scope
    var repo1 = sp.GetRequiredService<ICustomerRepository>(); // resolve ICustomerRepository from the service provider
    var repo2 = sp.GetRequiredService<ICustomerRepository>(); // resolve ICustomerRepository from the service provider
    Console.WriteLine($"Iteration {i}: {repo1.InstanceId} - {repo2.InstanceId}"); // print the unique identifier of the repository instance

    // add customer from repo1 then get it from repo2 to demonstrate that they are the same instance
    var customer = new MiniCrm.Domain.Entities.Customer { Id = Guid.NewGuid(), Name = $"Customer {i}", Email = $"customer{i}@example.com" };
    await repo1.AddAsync(customer, CancellationToken.None); // add customer to repo1
    var retrievedCustomer = await repo2.GetByIdAsync(customer.Id, CancellationToken.None); // get customer from repo2
    Console.WriteLine($"Iteration {i}: Retrieved customer - {retrievedCustomer?.Name}"); // print the name of the retrieved customer
    var count = await repo1.CountAsync(CancellationToken.None); // get the count of customers from repo1
    Console.WriteLine($"Iteration {i}: Count of customers - {count}"); // print
}

host.Run(); // run the host, which will start the Worker and block until shutdown
