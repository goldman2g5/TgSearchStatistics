namespace TgSearchStatistics.Models.GetModels
{
    public class SubscriptionModel
    {
        public int SubscriptionId { get; set; }
        public int ChannelId { get; set; }
        public string? ChannelName { get; set; }
        public int SubscriptionTypeId { get; set; }
        public string? SubscriptionTypeName { get; set; }
        public DateTime? ExpirationDate { get; set; }
        // Add other fields that represent subscription details
    }
}
