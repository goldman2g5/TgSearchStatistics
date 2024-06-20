namespace TgSearchStatistics.Models.PostModels
{
    public class CreatePaymentRequest
    {
        public int SubscriptionTypeId { get; set; }
        public int Duration { get; set; }
        public bool AutoRenewal { get; set; }
        public int Discount { get; set; }
        public int ChannelId { get; set; }
        public string ChannelName { get; set; } = null!;
    }
}
