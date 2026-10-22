namespace MiniCrm.Worker;
public static partial class Log
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Customer added successfully: {CustomerId}")]
    public static partial void LogCustomerAdded(this ILogger logger, Guid customerId);
    [LoggerMessage(EventId = 2, Level = LogLevel.Warning, Message = "Failed to add customer: {CustomerId}")]
    public static partial void LogCustomerAddFailed(this ILogger logger, Guid customerId);
    [LoggerMessage(EventId = 3, Level = LogLevel.Debug, Message = "Worker iteration started at {Timestamp}")]
    public static partial void LogWorkerIteration(this ILogger logger, DateTimeOffset timestamp);
}