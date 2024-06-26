namespace TgSearchStatistics.Models.BaseModels;

public partial class JobScheduleRecord
{
    public int Id { get; set; }

    public string JobName { get; set; } = null!;

    public DateOnly? LastRun { get; set; }

    public string? Status { get; set; }
}
