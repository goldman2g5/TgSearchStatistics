using WTelegram;

namespace TgSearchStatistics.Models
{
    public class TelegramClientWrapper
    {
        public Client Client { get; set; }
        public int DatabaseId { get; set; } // Database record ID of the TgClient

        public long TelegramId { get; set; }

        public bool IsBusy { get; set; }

        public TelegramClientWrapper(Client client, int databaseId, long telegramId)
        {
            Client = client;
            DatabaseId = databaseId;
            TelegramId = telegramId;
            IsBusy = false;
        }
    }
}
