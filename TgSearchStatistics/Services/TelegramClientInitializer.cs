using TgCheckerApi.Controllers;
using TgSearchStatistics.Models;

namespace TgSearchStatistics.Services
{
    public class TelegramClientInitializer : IHostedService
    {
        private readonly TelegramClientService _telegramClientService;
        private readonly ILogger<TelegramClientWrapper> _logger;


        public TelegramClientInitializer(TelegramClientService telegramClientService, ILogger<TelegramClientWrapper> logger)
        {
            _telegramClientService = telegramClientService;
            TelegramClientWrapper.SetLogger(logger);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _telegramClientService.InitializeAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
