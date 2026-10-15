namespace MiniCrm.Worker.Notification
{
    public interface INotificationSender
    {
        Task SendAsync(string recipient, string subject, string message, CancellationToken cancellationToken = default);
    }
}