using MiniCrm.Domain.Abstractions;
using MiniCrm.Domain.Repository;
using MiniCrm.Domain.Services;
using MiniCrm.Domain.Validation;
using MiniCrm.Worker;
using MiniCrm.Worker.Notification;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args); // config + logging + DI

if (builder.Environment.IsDevelopment())
{
    var root = (IConfigurationRoot)builder.Configuration;
    Console.WriteLine($"[config] ConnectionStrings:MiniCrm => {root.GetConnectionString("MiniCrm")}"); // get the connection string for MiniCrm
    
    foreach(var provider in root.Providers.Reverse())
    {
        if (provider.TryGet("MiniCrm:PageSize", out var value))
        {
            Console.WriteLine($"[config] {provider} => {value}"); // log the provider and value for MiniCrm:PageSize
        }
        else
        {
            Console.WriteLine($"[config] {provider} => MiniCrm:PageSize not found");
        }
    }
}

builder.Services
    .AddOptions<MiniCrmOptions>()
    .BindConfiguration(MiniCrmOptions.SectionName,
        binder => binder.ErrorOnUnknownConfiguration = true) // bind MiniCrmOptions to configuration section
    .ValidateDataAnnotations(); // validate MiniCrmOptions using data annotations
    //.ValidateOnStart(); // validate MiniCrmOptions on startup

builder.Services.AddHostedService<ConfigDemoWorker>(); // register ConfigDemoWorker as a Singleton IHostedService
builder.Services.AddMiniCrmDomain(); // register domain services and repositories

builder.Services.AddKeyedSingleton<INotificationSender, EmailNotificationSender>(NotificationChannels.Email); // register INotificationSender with its implementation  
builder.Services.AddKeyedSingleton<INotificationSender, SmsNotificationSender>(NotificationChannels.Sms); // register INotificationSender with its implementation  

builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var defaultChannel = configuration.GetValue<string>("NotificationChannels:Default");
    var key = defaultChannel?.Trim().ToLowerInvariant() switch
    {
        "email" => NotificationChannels.Email,
        "sms"   => NotificationChannels.Sms,
        _ => throw new InvalidOperationException(
                 $"NotificationChannels:Default channel is invalid: '{defaultChannel}'. Only 'Email' or 'Sms' are allowed.")
    };
    return sp.GetRequiredKeyedService<INotificationSender>(key); // resolve the INotificationSender based on the default channel

}); // register INotificationSender with its implementation based on configuration

//builder.Services.AddScoped<IServiceScopeFactory, IServiceScopeFactory>(); // register IServiceScopeFactory with its implementation
//builder.Services.AddHostedService<LifecycleDemo>(); // register LifecycleDemo as a Singleton IHostedService

//builder.Services.AddHostedService<SeedCustomer>(); // register SeedCustomer as a Singleton IHostedService
//builder.Services.AddHostedService<Worker>(); // register Worker as a Singleton IHostedService

//builder.Services.AddScoped<IAuditSessionService, AuditSessionService>(); // register IAuditSessionService with its implementation
//builder.Services.AddSingleton<IAuditSessionService, AuditSessionService>(); // register IAuditSessionService with its implementation
builder.Services.AddSingleton<IAuditSessionService>(sp => new AuditSessionService(sp.GetRequiredService<ILogger<AuditSessionService>>())); // register IAuditSessionService with its implementation
// builder.Services.Configure<HostOptions>(hostOptions =>
// {
//     hostOptions.ShutdownTimeout = TimeSpan.FromSeconds(10); // set the shutdown timeout to 10 seconds
// });


var host = builder.Build(); // lock container and create the host

//host.Services.GetRequiredService<IAuditSessionService>(); // resolve IAuditSessionService to ensure it is created

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
    Console.WriteLine($"Iteration {i}: Count of customers - {count}"); // print the count of customers
}

host.Run(); // run the host, which will start the Worker and block until shutdown
