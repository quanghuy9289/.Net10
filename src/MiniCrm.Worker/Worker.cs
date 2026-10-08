using MiniCrm.Domain.Abstractions;

namespace MiniCrm.Worker;

public class Worker(ICustomerRepository customerRepository, ILogger<Worker> logger) : BackgroundService // a hosted service that runs in the background
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
                var count = await customerRepository.CountAsync(stoppingToken); // get the count of customers from the repository
                //var count = await customerRepository.CountAsync(CancellationToken.None); // get the count of customers from the repository
                logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Customer count: {count}"); // log the count
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
