namespace TgSearchStatistics.Models.TempScaffold;

public partial class NotificationType
{
    public int Id { get; set; }

    public string Text { get; set; } = null!;

    public virtual ICollection<NotificationDelayedTask> NotificationDelayedTasks { get; set; } = new List<NotificationDelayedTask>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
