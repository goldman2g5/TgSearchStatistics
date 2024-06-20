using Microsoft.EntityFrameworkCore;
using TgSearchStatistics.Models.BaseModels;
using TgSearchStatistics.Models.DTO;

namespace TgSearchStatistics.Services
{
    public class TgClientFactory
    {
        private readonly IDbContextFactory<TgDbContext> _dbContextFactory;
        private readonly IWebHostEnvironment _env;

        public TgClientFactory(IDbContextFactory<TgDbContext> dbContextFactory, IWebHostEnvironment env)
        {
            _dbContextFactory = dbContextFactory;
            _env = env;
        }

        public async Task<IEnumerable<TelegramClientInitDto>> FetchClientConfigsAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var tgClients = await dbContext.TgClients.ToListAsync();
            var clientConfigs = tgClients.Select(tgClient => new TelegramClientInitDto
            {
                Config = (config) =>
                {
                    return config switch
                    {
                        "api_id" => tgClient.ApiId,
                        "api_hash" => tgClient.ApiHash,
                        "session_pathname" => Path.Combine(_env.ContentRootPath, "WTelegramSessions", $"WTelegram_{tgClient.Id}.session"),
                        _ => null,
                    };
                },
                PhoneNumber = tgClient.PhoneNumber,
                DatabaseId = tgClient.Id,
                TelegramId = (long)tgClient.TelegramId
            });

            return clientConfigs;
        }
    }
}
