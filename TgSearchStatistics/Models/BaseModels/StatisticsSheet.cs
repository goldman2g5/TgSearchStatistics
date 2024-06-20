using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TgSearchStatistics.Models.BaseModels;

public partial class StatisticsSheet
{
    public int Id { get; set; }

    public int ChannelId { get; set; }

    public virtual Channel Channel { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<MonthViewsRecord> MonthViewsRecords { get; set; } = new List<MonthViewsRecord>();
    [JsonIgnore]
    public virtual ICollection<SubscribersRecord> SubscribersRecords { get; set; } = new List<SubscribersRecord>();
    [JsonIgnore]
    public virtual ICollection<ViewsRecord> ViewsRecords { get; set; } = new List<ViewsRecord>();
}
