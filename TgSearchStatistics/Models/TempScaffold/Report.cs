using System;
using System.Collections.Generic;

namespace TgSearchStatistics.Models.TempScaffold;

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

    public int? CommentId { get; set; }

    public int? ReportType { get; set; }

    public virtual Channel Channel { get; set; } = null!;

    public virtual Comment? Comment { get; set; }

    public virtual ReportType? ReportTypeNavigation { get; set; }

    public virtual Staff? Staff { get; set; }

    public virtual User User { get; set; } = null!;
}
