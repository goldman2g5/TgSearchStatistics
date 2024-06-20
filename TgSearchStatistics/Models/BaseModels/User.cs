using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TgSearchStatistics.Models.BaseModels;

namespace TgSearchStatistics.Models.BaseModels;

public partial class User
{
    public int Id { get; set; }

    public long? TelegramId { get; set; }

    public long? ChatId { get; set; }

    public string Username { get; set; } = null!;

    public byte[]? Avatar { get; set; }

    public string? UniqueKey { get; set; }

    public int? NotificationSettings { get; set; }

    public DateTime? LastUpdate { get; set; }
    [JsonIgnore]
    public virtual ICollection<ChannelAccess> ChannelAccesses { get; set; } = new List<ChannelAccess>();
    [JsonIgnore]
    public virtual ICollection<Channel> Channels { get; set; } = new List<Channel>();
    [JsonIgnore]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    [JsonIgnore]
    public virtual ICollection<NotificationDelayedTask> NotificationDelayedTasks { get; set; } = new List<NotificationDelayedTask>();

    [JsonIgnore]
    public virtual NotificationSetting? NotificationSettingsNavigation { get; set; }
    [JsonIgnore]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    [JsonIgnore]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    [JsonIgnore]
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
    [JsonIgnore]
    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
    [JsonIgnore]
    public virtual ICollection<TelegramPayment> TelegramPayments { get; set; } = new List<TelegramPayment>();
}
