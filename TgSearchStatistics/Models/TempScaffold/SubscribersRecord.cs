using System;
using System.Collections.Generic;

namespace TgSearchStatistics.Models.TempScaffold;

public partial class SubscribersRecord
{
    public int Id { get; set; }

    public int Subscribers { get; set; }

    public DateTime Date { get; set; }

    public int Sheet { get; set; }

    public virtual StatisticsSheet SheetNavigation { get; set; } = null!;
}
