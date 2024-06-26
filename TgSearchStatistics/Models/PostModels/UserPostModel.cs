using TgSearchStatistics.Models.BaseModels;

namespace TgSearchStatistics.Models
{
    public class UserPostModel : User
    {
        [System.Text.Json.Serialization.JsonIgnore]
        new public virtual ICollection<ChannelAccess> ChannelAccesses { get; set; } = new List<ChannelAccess>();

        [System.Text.Json.Serialization.JsonIgnore]
        new public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        [System.Text.Json.Serialization.JsonIgnore]
        new public string? UniqueKey
        {
            get { return base.UniqueKey; }
            set { base.UniqueKey = value; }
        }
    }
}
