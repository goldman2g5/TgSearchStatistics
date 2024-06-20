namespace TgSearchStatistics.Models.DTO
{
    public class TelegramClientInitDto
    {
        public Func<string, string> Config { get; set; }
        public string PhoneNumber { get; set; }

        public int DatabaseId { get; set; }

        public long TelegramId { get; set; }
    }
}
