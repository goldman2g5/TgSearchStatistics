using System;
using System.Collections.Generic;

namespace TgSearchStatistics.Models.TempScaffold;

public partial class NotificationDelayedTask
{
    public int Id { get; set; }

    public int? ChannelId { get; set; }

    public DateTime Date { get; set; }

    public string? Content { get; set; }

    public int TypeId { get; set; }

    public string ContentType { get; set; } = null!;

    public bool TargetTelegram { get; set; }

    public int UserId { get; set; }

    public virtual Channel? Channel { get; set; }

    public virtual NotificationType Type { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
