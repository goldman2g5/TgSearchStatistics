using System.Text.Json.Serialization;
using TgSearchStatistics.Models.BaseModels;

namespace TgSearchStatistics.Models.PostModels
{
    public class ReportPostModel : Report
    {
        [JsonIgnore]
        new public virtual int UserId
        {
            get { return base.UserId; }
            set { base.UserId = (int)value; }
        }
        [JsonIgnore]
        new public virtual int ChannelId
        {
            get { return base.ChannelId; }
            set { base.ChannelId = value; }
        }

        [JsonIgnore]
        new public virtual DateTime? ReportTime
        {
            get { return base.ReportTime; }
            set { base.ReportTime = value; }
        }
        [JsonIgnore]
        new public virtual Channel? Channel { get; set; } = null!;

        [JsonIgnore]
        new public string? Status
        {
            get { return base.Status; }
            set { base.Status = value; }
        }

        [JsonIgnore]
        new public int? StaffId
        {
            get { return base.StaffId; }
            set { base.StaffId = value; }
        }

        [JsonIgnore]
        new public bool? NotificationSent
        {
            get { return base.NotificationSent; }
            set { base.NotificationSent = value; }
        }

        [JsonIgnore]
        new public virtual User? User { get; set; } = null!;
        [JsonIgnore]
        new public virtual Staff? Staff { get; set; }

    }
}
