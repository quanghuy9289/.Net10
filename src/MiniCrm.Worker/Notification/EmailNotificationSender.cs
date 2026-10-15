namespace MiniCrm.Worker.Notification
{
    public sealed class EmailNotificationSender(ILogger<EmailNotificationSender> logger) : INotificationSender
    {

        public Task SendAsync(string recipient, string subject, string message, CancellationToken cancellationToken = default)
        {
            // Simulate sending an email notification
            logger.LogInformation("Sending email to {Recipient} with subject '{Subject}' and message '{Message}'", recipient, subject, message);
            return Task.CompletedTask;
        }
    }

    
}

