using TgSearchStatistics.Models.BaseModels;

namespace TgSearchStatistics.Models.GetModels
{
    public class ReplyGetModel : Comment
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public int UserId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public int ChannelId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public int? ParentId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public int? Rating { get; set; }

        public string Username { get; set; }
    }
}
