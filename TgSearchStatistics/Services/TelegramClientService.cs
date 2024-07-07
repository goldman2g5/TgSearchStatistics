using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using TgSearchStatistics.Models;
using TgSearchStatistics.Models.BaseModels;
using TgSearchStatistics.Utility;
using TL;
using WTelegram;

namespace TgSearchStatistics.Services
{
    public class TelegramClientService
    {
        private readonly TgClientFactory _tgClientFactory;
        private readonly IDbContextFactory<TgDbContext> _dbContextFactory;
        private readonly ILogger<TelegramClientService> _logger;
        private readonly ILogger<TaskDispatcher> _dispatcherLogger;
        public static List<TelegramClientWrapper> Clients { get; private set; } = new List<TelegramClientWrapper>();
        private readonly TaskDispatcher _taskDispatcher; // Add TaskDispatcher field

        public TelegramClientService(TgClientFactory tgClientFactory, ILogger<TelegramClientService> logger, ILogger<TaskDispatcher> dispatcherLogger, IDbContextFactory<TgDbContext> dbContextFactory)
        {
            _tgClientFactory = tgClientFactory;
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _dispatcherLogger = dispatcherLogger;
            _taskDispatcher = new TaskDispatcher(_dispatcherLogger); // Initialize TaskDispatcher
        }

        public async Task InitializeAsync()
        {
            var clientConfigs = await _tgClientFactory.FetchClientConfigsAsync();
            var httpClient = new HttpClient { BaseAddress = new Uri("http://127.0.0.1:8000/") };

            foreach (var clientConfig in clientConfigs)
            {
                var client = new Client(clientConfig.Config);
                var loginInfo = clientConfig.PhoneNumber;

                while (client.User == null)
                {
                    switch (await client.Login(loginInfo))
                    {
                        case "verification_code":
                            var userId = clientConfig.TelegramId;
                            await httpClient.PostAsync($"/trigger_verification/{userId}", new StringContent(""));


                            HttpResponseMessage response;
                            string verificationCode = null;
                            do
                            {
                                try
                                {
                                    await Task.Delay(5000);
                                    response = await httpClient.GetAsync($"/get_verification_code/{userId}");
                                    Console.WriteLine(userId);
                                    Console.WriteLine(response.StatusCode);
                                    if (response.IsSuccessStatusCode)
                                    {
                                        var content = await response.Content.ReadAsStringAsync();
                                        var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                                        verificationCode = data?["verification_code"];
                                        Console.WriteLine(verificationCode);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Exception whili logging in {ex.Message}");
                                };


                            } while (verificationCode == null);

                            if (verificationCode != null)
                            {
                                loginInfo = verificationCode;
                            }
                            else
                            {
                                Console.WriteLine("Failed to get verification code.");
                                return;
                            }
                            break;
                        default:
                            loginInfo = null; break;
                    }
                }
                Console.WriteLine($"We are logged-in as {client.User} (id {client.User.id})");

                Clients.Add(new TelegramClientWrapper(client, clientConfig.DatabaseId, clientConfig.TelegramId));
            }
        }

        public async Task<Client?> GetClient()
        {
            var clientWrapper = Clients.FirstOrDefault();
            return clientWrapper?.Client;
        }

        public async Task<Client?> GetClientByDatabaseId(int databaseId)
        {
            var clientWrapper = Clients.FirstOrDefault(c => c.DatabaseId == databaseId);
            return clientWrapper?.Client;
        }

        public async Task<Client?> GetClientByTelegramId(long telegramChannelId)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();

            var pyrogramChannelId = TelegramUtility.ToPyrogram(telegramChannelId);

            var dbChannel = await dbContext.Channels
                .Where(c => c.TelegramId == pyrogramChannelId)
                .FirstOrDefaultAsync();

            if (dbChannel == null)
            {
                _logger.LogInformation($"Channel with Telegram ID {telegramChannelId} not found in database.");
                return null;
            }

            var clientWrapper = Clients.FirstOrDefault(c => c.DatabaseId == dbChannel.TgclientId);
            if (clientWrapper == null)
            {
                var clientsDbIds = Clients.Select(x => x.DatabaseId).ToList();
                if (clientsDbIds.Count == 0)
                {
                    _logger.LogInformation("No clients available to join the channel.");
                    return null;
                }

                foreach (var client in Clients)
                {
                    _logger.LogInformation($"Client {client.DatabaseId}: Total busy time: {client.GetTotalBusyTime().TotalSeconds} seconds.");
                }

                clientWrapper = Clients.OrderBy(c => c.GetTotalBusyTime())
                                       .FirstOrDefault();

                if (clientWrapper == null)
                {
                    _logger.LogInformation("No matching clients found based on load time.");
                    return null;
                }

                _logger.LogInformation($"Selected Client {clientWrapper.DatabaseId} with the lowest busy time: {clientWrapper.GetTotalBusyTime().TotalSeconds} seconds.");
            }

            return clientWrapper.Client;
        }

        public async Task<List<TL.Message>> GetMessagesByPeriodAsync(long channelId, DateTime startDate, DateTime endDate)
        {
            var tcs = new TaskCompletionSource<List<TL.Message>>();

            var taskRequest = new TaskRequest
            {
                ChannelId = channelId,
                ChannelUsername = await GetChannelUsername(channelId),
                TaskToExecute = async client =>
                {
                    var messages = await GetMessagesByPeriodInternalAsync(client, channelId, startDate, endDate);
                    return messages;
                },
                TaskCompletionSource = tcs
            };

            _taskDispatcher.EnqueueTask(taskRequest); // Enqueue the task to the dispatcher

            return await tcs.Task;
        }

        private async Task<List<TL.Message>> GetMessagesByPeriodInternalAsync(Client client, long channelId, DateTime startDate, DateTime endDate)
        {
            var allMessages = new List<TL.Message>();
            var channelInfo = await GetChannelAccessHash(channelId, client);
            if (!channelInfo.HasValue)
            {
                throw new InvalidOperationException("Channel not found or access hash unavailable.");
            }

            var (resolvedChannelId, accessHash) = channelInfo.Value;
            var inputPeer = new InputPeerChannel(resolvedChannelId, accessHash);

            int limit = 100;
            DateTime offsetDate = DateTime.UtcNow;
            int lastMessageId = 0;

            while (offsetDate > startDate)
            {
                var messagesBatch = await client.Messages_GetHistory(inputPeer, offset_id: lastMessageId, limit: limit, offset_date: offsetDate);
                var messages = messagesBatch.Messages.OfType<TL.Message>()
                    .Where(m => m.Date >= startDate && m.Date <= endDate)
                    .ToList();

                if (messages.Count == 0)
                {
                    break;
                }

                allMessages.AddRange(messages);
                var earliestMessageInBatch = messages.OrderBy(m => m.Date).FirstOrDefault();

                if (earliestMessageInBatch == null || earliestMessageInBatch.Date <= startDate)
                {
                    break;
                }

                offsetDate = earliestMessageInBatch.Date;
                lastMessageId = earliestMessageInBatch.id;
            }

            return allMessages;
        }

        private async Task<string> GetChannelUsername(long channelId)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var pyrogramChannelId = TelegramUtility.ToPyrogram(channelId);

            var dbChannel = await dbContext.Channels
                .Where(c => c.TelegramId == pyrogramChannelId)
                .FirstOrDefaultAsync();

            if (dbChannel == null)
            {
                throw new InvalidOperationException($"Channel with Telegram ID {channelId} not found in database.");
            }

            return TelegramUtility.RemoveTMeUrl(dbChannel.Url); // Assuming URL contains the username
        }

        public async Task<(long channelId, long accessHash)?> GetChannelAccessHash(long telegramChannelId, Client client)
        {
            try
            {
                if (client == null)
                {
                    _logger.LogError("Failed to get Telegram client.");
                    return null;
                }

                using var dbContext = _dbContextFactory.CreateDbContext();
                var pyrogramChannelId = TelegramUtility.ToPyrogram(telegramChannelId);

                var dbChannel = await dbContext.Channels
                    .Where(c => c.TelegramId == pyrogramChannelId)
                    .FirstOrDefaultAsync();

                if (dbChannel == null)
                {
                    _logger.LogError($"Channel with Telegram ID {telegramChannelId} not found in the database.");
                    return null;
                }

                var channelUsername = TelegramUtility.RemoveTMeUrl(dbChannel.Url);

                var resolveResult = await client.Contacts_ResolveUsername(channelUsername);
                var channel = resolveResult.Channel;

                if (channel != null)
                {
                    _logger.LogInformation($"Successfully resolved channel: {channelUsername} with access hash {channel.access_hash}");
                    return (channel.ID, channel.access_hash);
                }
                else
                {
                    _logger.LogError($"Failed to resolve channel or mismatch in channel ID for username: {channelUsername}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while trying to resolve the channel access hash: {ex.Message}");
            }

            return null;
        }

        public static void DisposeClients()
        {
            foreach (var client in Clients)
            {
                if (client is IDisposable disposableClient)
                {
                    disposableClient.Dispose();
                }
            }
        }
    }
}
