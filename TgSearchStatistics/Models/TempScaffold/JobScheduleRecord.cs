using System;
using System.Collections.Generic;

namespace TgSearchStatistics.Models.TempScaffold;

public partial class JobScheduleRecord
{
    public int Id { get; set; }

    public string JobName { get; set; } = null!;

    public DateOnly? LastRun { get; set; }

    public string? Status { get; set; }
}
