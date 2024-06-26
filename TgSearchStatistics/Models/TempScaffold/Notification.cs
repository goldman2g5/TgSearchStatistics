namespace TgSearchStatistics.Models.TempScaffold;

public partial class Notification
{
    public int Id { get; set; }

    public int ChannelId { get; set; }

    public DateTime Date { get; set; }

    public string Content { get; set; } = null!;

    public bool IsNew { get; set; }

    public int TypeId { get; set; }

    public int UserId { get; set; }

    public string? ContentType { get; set; }

    public bool? TargetTelegram { get; set; }

    public virtual Channel Channel { get; set; } = null!;

    public virtual NotificationType Type { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
