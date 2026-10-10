using MiniCrm.Domain.Abstractions;
using MiniCrm.Domain.Common;
using MiniCrm.Domain.Entities;
using MiniCrm.Domain.Validation;

namespace MiniCrm.Worker;

public class Worker(IServiceScopeFactory serviceScopeFactory, ILogger<Worker> logger) : BackgroundService // a hosted service that runs in the background
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromSeconds(3); // run every 3 seconds
        using var timer = new PeriodicTimer(interval); // create a timer that ticks every interval

        logger.LogInformation("Worker started. Logging customer count every {Interval} seconds.", interval.TotalSeconds); // log that the worker has started

        while (await timer.WaitForNextTickAsync(stoppingToken)) // wait for the next tick, or exit if cancellation is requested
        {
            try
            {
                logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Counting customers..."); // log that we are counting customers
                
                await using var scope = serviceScopeFactory.CreateAsyncScope(); // create a new scope
                var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>(); // resolve ICustomerService from the service provider
                var count = await customerService.CountCustomerAsync(stoppingToken); // get the count of customers from the service
                //var count = await customerService.CountAsync(CancellationToken.None); // get the count of customers from the service

                Result<Customer> addResult = await customerService.AddCustomerAsync(new CustomerRequest { Name = "Worker Customer", Email = "worker" }, CancellationToken.None);
                
                if (!addResult.IsSuccess)
                {
                    logger.LogWarning("Failed to add customer: {Errors}", addResult.Errors.Select(e => $"{e.Field}: {e.Message}").ToArray()); // log the validation errors
                }
                else if (addResult is Result<Customer> successResult)
                {
                    logger.LogInformation("Successfully added customer: {CustomerId}", successResult.Value.Id);
                    logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Customer count: {count}"); // log the count
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) // if cancellation is requested, log and exit
            {
                logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Worker is stopping due to cancellation."); // log that the worker is stopping
                break; // exit the loop if cancellation is requested
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while counting customers."); // log any errors that occur
            }
        }
    }
}
