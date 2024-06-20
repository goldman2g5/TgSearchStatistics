namespace TgSearchStatistics.Models.NotificationModels
{
    public class TelegramNotificationPostModel
    {
        public string ChannelName { get; set; }

        public int? ChannelId { get; set; }

        public int UserId { get; set; }

        public long? TelegramUserId { get; set; }

        public long? TelegramChatId { get; set; }

        public long? TelegamChannelId { get; set; }

        public string ContentType { get; set; }

        public string UniqueKey { get; set; }
    }
}
