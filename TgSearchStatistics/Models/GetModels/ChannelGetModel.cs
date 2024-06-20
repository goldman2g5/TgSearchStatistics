using TgSearchStatistics.Models.BaseModels;

namespace TgSearchStatistics.Models.GetModels
{
    public class ChannelGetModel : Channel
    {
        public List<string> Tags { get; set; }

        public string urlCut { get; set; }

        public int? subType { get; set; }

        public DateTime? SubscriptionExpirationDate { get; set; }
    }
}
