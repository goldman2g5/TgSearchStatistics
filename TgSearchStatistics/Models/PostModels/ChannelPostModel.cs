using System.Text.Json.Serialization;
using TgSearchStatistics.Models.BaseModels;

namespace TgSearchStatistics.Models
{
    public class ChannelPostModel : Channel
    {
        public long userTelegramID { get; set; }

        [JsonIgnore]
        new public virtual ICollection<ChannelAccess> ChannelAccesses { get; set; } = new List<ChannelAccess>();
        [JsonIgnore]
        new public virtual ICollection<ChannelHasSubscription> ChannelHasSubscriptions { get; set; } = new List<ChannelHasSubscription>();
        [JsonIgnore]
        new public virtual ICollection<ChannelHasTag> ChannelHasTags { get; set; } = new List<ChannelHasTag>();
        [JsonIgnore]
        new public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        [JsonIgnore]
        new public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
