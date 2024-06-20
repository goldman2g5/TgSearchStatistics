namespace TgSearchStatistics.Models.DTO
{
    public class NotificationCreateRequest
    {
        public int ChannelId { get; set; }
        public string Content { get; set; }
        public int TypeId { get; set; }
        public int UserId { get; set; }
        public bool TargetTelegram { get; set; } = false;
        public string ContentType { get; set; }
        public string ChannelName { get; set; }
        public long? TelegramUserId { get; set; }
        public long? TelegramChatId { get; set; }
        public long? TelegramChannelId { get; set; }
    }
}
