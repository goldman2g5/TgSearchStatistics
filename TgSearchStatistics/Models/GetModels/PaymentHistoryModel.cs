namespace TgSearchStatistics.Models.GetModels
{
    public class PaymentHistoryModel
    {
        public Guid PaymentId { get; set; }
        public int ChannelId { get; set; }
        public int ChannelSubscriptionType { get; set; } // Adjust the type as needed
        public DateTime? PurchaseDate { get; set; }
        public int SubscriptionDuration { get; set; }
        public decimal? AmountValue { get; set; }
        public string AmountCurrency { get; set; }
    }
}
