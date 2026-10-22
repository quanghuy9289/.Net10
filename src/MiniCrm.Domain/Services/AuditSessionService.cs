using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MiniCrm.Domain.Abstractions;

public sealed class AuditSessionService : IAuditSessionService
{
    private readonly ILogger<AuditSessionService> _logger;

    public AuditSessionService(ILogger<AuditSessionService> logger)
    {
        _logger = logger;
        _logger.LogInformation("Audit session started - {AuditSessionId}", this.GetHashCode());
    }


    public async ValueTask DisposeAsync()
    {
        _logger.LogInformation("Audit session ended - {AuditSessionId}", this.GetHashCode());
        await Task.CompletedTask;
    }
}