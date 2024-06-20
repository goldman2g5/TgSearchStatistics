namespace TgSearchStatistics.Services
{
    public class TelegramClientInitializer : IHostedService
    {
        private readonly TelegramClientService _telegramClientService;

        public TelegramClientInitializer(TelegramClientService telegramClientService)
        {
            _telegramClientService = telegramClientService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _telegramClientService.InitializeAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
