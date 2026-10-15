using Microsoft.Extensions.Options;

namespace MiniCrm.Worker;
public sealed class ConfigDemoWorker(
    IOptions<MiniCrmOptions> options,
    IOptionsMonitor<MiniCrmOptions> monitor,
    IServiceScopeFactory scopeFactory,
    ILogger<ConfigDemoWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("ConfigDemoWorker is starting.");

        using var changeSubscription = monitor.OnChange((newOptions) =>
        {
            logger.LogInformation("Onchange fired: PageSize={PageSize}", newOptions.PageSize);
        });

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(3));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = scopeFactory.CreateScope();
            var snapshot = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<MiniCrmOptions>>();
            
            logger.LogInformation(
                "IOptions={Options} | Snapshot={Snapshot} | Monitor={Monitor}",
                options.Value.PageSize,
                snapshot.Value.PageSize,
                monitor.CurrentValue.PageSize);
        }

        logger.LogInformation("ConfigDemoWorker has stopped.");
    }
}