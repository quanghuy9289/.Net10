namespace MiniCrm.Worker.Notification
{
    public sealed class SmsNotificationSender(ILogger<SmsNotificationSender> logger) : INotificationSender
    {
        public Task SendAsync(string recipient, string subject, string message, CancellationToken cancellationToken = default)
        {
            // Simulate sending an SMS notification
            logger.LogInformation("Sending SMS to {Recipient} with subject '{Subject}' and message '{Message}'", recipient, subject, message);
            return Task.CompletedTask;
        }
    }
}