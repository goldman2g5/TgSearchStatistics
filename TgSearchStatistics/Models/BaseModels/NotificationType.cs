using Newtonsoft.Json;

namespace TgSearchStatistics.Models.BaseModels;

public partial class NotificationType
{
    public int Id { get; set; }

    public string Text { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<NotificationDelayedTask> NotificationDelayedTasks { get; set; } = new List<NotificationDelayedTask>();

    [JsonIgnore]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
