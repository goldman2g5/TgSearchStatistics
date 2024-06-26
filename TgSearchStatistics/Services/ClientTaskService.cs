using TgSearchStatistics.Models;
using WTelegram;

namespace TgSearchStatistics.Services
{
    public class ClientTaskService
    {
        public async Task ExecuteWithClientAsync(TelegramClientWrapper clientWrapper, Func<Client, Task> task)
        {
            if (clientWrapper == null)
            {
                throw new ArgumentNullException(nameof(clientWrapper));
            }

            try
            {
                clientWrapper.IsBusy = true;
                await task(clientWrapper.Client);
            }
            finally
            {
                clientWrapper.IsBusy = false;
            }
        }
    }
}
