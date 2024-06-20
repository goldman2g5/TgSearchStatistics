using System;
using System.Collections.Generic;

namespace TgSearchStatistics.Models.TempScaffold;

public partial class StatisticsSheet
{
    public int Id { get; set; }

    public int ChannelId { get; set; }

    public virtual Channel Channel { get; set; } = null!;

    public virtual ICollection<MonthViewsRecord> MonthViewsRecords { get; set; } = new List<MonthViewsRecord>();

    public virtual ICollection<SubscribersRecord> SubscribersRecords { get; set; } = new List<SubscribersRecord>();

    public virtual ICollection<ViewsRecord> ViewsRecords { get; set; } = new List<ViewsRecord>();
}
