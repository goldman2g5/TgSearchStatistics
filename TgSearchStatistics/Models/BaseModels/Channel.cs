﻿using Newtonsoft.Json;

namespace TgSearchStatistics.Models.BaseModels;

public partial class Channel
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int? Members { get; set; }

    public byte[]? Avatar { get; set; }

    public int? User { get; set; }

    public bool? Notifications { get; set; }

    public decimal? Bumps { get; set; }

    public DateTime? LastBump { get; set; }

    public long? TelegramId { get; set; }

    public bool? NotificationSent { get; set; }

    public bool? PromoPost { get; set; }

    public TimeOnly? PromoPostTime { get; set; }

    public int? PromoPostInterval { get; set; }

    public bool? PromoPostSent { get; set; }

    public DateTime? PromoPostLast { get; set; }

    public string? Language { get; set; }

    public string? Flag { get; set; }

    public string? Url { get; set; }

    public bool? Hidden { get; set; }

    public int? TopPos { get; set; }

    public bool? IsPartner { get; set; }

    public int? TgclientId { get; set; }

    [JsonIgnore]
    public virtual ICollection<ChannelAccess> ChannelAccesses { get; set; } = new List<ChannelAccess>();
    [JsonIgnore]
    public virtual ICollection<ChannelHasSubscription> ChannelHasSubscriptions { get; set; } = new List<ChannelHasSubscription>();
    [JsonIgnore]
    public virtual ICollection<ChannelHasTag> ChannelHasTags { get; set; } = new List<ChannelHasTag>();
    [JsonIgnore]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    [JsonIgnore]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    [JsonIgnore]
    public virtual ICollection<NotificationDelayedTask> NotificationDelayedTasks { get; set; } = new List<NotificationDelayedTask>();
    [JsonIgnore]
    public virtual ICollection<Notification> NotificationsNavigation { get; set; } = new List<Notification>();
    [JsonIgnore]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    [JsonIgnore]
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
    [JsonIgnore]
    public virtual ICollection<StatisticsSheet> StatisticsSheets { get; set; } = new List<StatisticsSheet>();
    [JsonIgnore]
    public virtual ICollection<TelegramPayment> TelegramPayments { get; set; } = new List<TelegramPayment>();

    [JsonIgnore]
    public virtual TgClient? Tgclient { get; set; }

    [JsonIgnore]
    public virtual User? UserNavigation { get; set; }
}
