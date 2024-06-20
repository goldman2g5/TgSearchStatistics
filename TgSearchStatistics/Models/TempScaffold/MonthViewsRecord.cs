using System;
using System.Collections.Generic;

namespace TgSearchStatistics.Models.TempScaffold;

public partial class MonthViewsRecord
{
    public int Id { get; set; }

    public int Views { get; set; }

    public DateTime Date { get; set; }

    public DateTime Updated { get; set; }

    public int Sheet { get; set; }

    public long? LastMessageId { get; set; }

    public virtual StatisticsSheet SheetNavigation { get; set; } = null!;
}
