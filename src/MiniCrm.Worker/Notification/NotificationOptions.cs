using System.ComponentModel.DataAnnotations;

namespace MiniCrm.Worker.Notification
{
    public sealed class NotificationOptions
    {
        public const string SectionName = "NotificationChannels";

        [Required, AllowedValues("Email", "Sms")]
        public string Default { get; set; } = "Email";
    }
}