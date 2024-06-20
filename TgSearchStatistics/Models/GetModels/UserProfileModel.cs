using System.Net;

namespace TgSearchStatistics.Models.GetModels
{
    public class UserProfileModel
    {
        public IList<ChannelGetModel> Channels { get; set; }

        public IList<CommentUserProfileGetModel> Comments { get; set; }

        public string SubType { get; set; }

        public string UserName { get; set; }

        public int UserId { get; set; }

        public byte[]? Avatar { get; set; }
    }
}
