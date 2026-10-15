using System.ComponentModel.DataAnnotations;

namespace MiniCrm.Worker;
public sealed class MiniCrmOptions
{
    public const string SectionName = "MiniCrm";

    [Range(1, 200)]
    public int PageSize { get; set; } = 10;
    public bool EnableNotifications { get; set; } = false;

    [Required, Url]
    public string NotificationEndpoint { get; set; } = string.Empty;
}