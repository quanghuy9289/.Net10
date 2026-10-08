using MiniCrm.Domain.Abstractions;
using MiniCrm.Domain.Repository;
using MiniCrm.Worker;

var builder = Host.CreateApplicationBuilder(args); // config + logging + DI

builder.Services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>(); // register ICustomerRepository with its implementation
builder.Services.AddHostedService<LifecycleDemo>(); // register LifecycleDemo as a Singleton IHostedService

builder.Services.AddHostedService<SeedCustomer>(); // register SeedCustomer as a Singleton IHostedService
builder.Services.AddHostedService<Worker>(); // register Worker as a Singleton IHostedService

// builder.Services.Configure<HostOptions>(hostOptions =>
// {
//     hostOptions.ShutdownTimeout = TimeSpan.FromSeconds(10); // set the shutdown timeout to 10 seconds
// });


var host = builder.Build(); // lock container and create the host
host.Run(); // run the host, which will start the Worker and block until shutdown
