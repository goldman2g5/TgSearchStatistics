using Newtonsoft.Json;

namespace TgSearchStatistics.Models.BaseModels;

public partial class Report
{
    public int Id { get; set; }

    public int ChannelId { get; set; }

    public int UserId { get; set; }

    public DateTime? ReportTime { get; set; }

    public string? Text { get; set; }

    public string? Reason { get; set; }

    public bool? NotificationSent { get; set; }

    public string? Status { get; set; }

    public int? StaffId { get; set; }

    public int? ReportType { get; set; }

    public int? CommentId { get; set; }
    [JsonIgnore]
    public virtual Channel Channel { get; set; } = null!;
    [JsonIgnore]
    public virtual Comment? Comment { get; set; }
    [JsonIgnore]
    public virtual ReportType? ReportTypeNavigation { get; set; }
    [JsonIgnore]
    public virtual Staff? Staff { get; set; }
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
