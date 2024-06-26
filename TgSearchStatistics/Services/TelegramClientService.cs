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
        public static List<TelegramClientWrapper> Clients { get; private set; } = new List<TelegramClientWrapper>();

        public TelegramClientService(TgClientFactory tgClientFactory, IDbContextFactory<TgDbContext> dbContextFactory)
        {
            _tgClientFactory = tgClientFactory;
            _dbContextFactory = dbContextFactory;
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
                            // Trigger the bot to ask for the verification code via FastAPI
                            var userId = clientConfig.TelegramId;
                            await httpClient.PostAsync($"/trigger_verification/{userId}", new StringContent(""));


                            // Wait for the verification code to be sent by the user
                            HttpResponseMessage response;
                            string verificationCode = null;
                            do
                            {
                                try
                                {
                                    await Task.Delay(5000); // Check every 5 seconds
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
                                return; // Handle the failure appropriately
                            }
                            break;
                        default:
                            loginInfo = null; break;
                    }
                }
                Console.WriteLine($"We are logged-in as {client.User} (id {client.User.id})");

                // Store the client with its database ID
                Clients.Add(new TelegramClientWrapper(client, clientConfig.DatabaseId, clientConfig.TelegramId));
            }

            await SyncClientsToChannelsAsync();
        }

        public async Task SyncClientsToChannelsAsync()
        {
            foreach (var wrapper in Clients)
            {
                var chats = await wrapper.Client.Messages_GetAllChats();
                using var dbContext = _dbContextFactory.CreateDbContext();

                // Use a HashSet to track unique channel IDs to avoid duplicates
                HashSet<long> uniqueChannelIds = new HashSet<long>();

                foreach (var (id, chat) in chats.chats)
                {
                    if (!chat.IsActive) continue;
                    var pyrogramChannelId = TelegramIdConverter.ToPyrogram(id);
                    uniqueChannelIds.Add(pyrogramChannelId);

                    var channel = await dbContext.Channels
                                    .Where(c => c.TelegramId == pyrogramChannelId)
                                    .FirstOrDefaultAsync();

                    if (channel != null)
                    {
                        channel.TgclientId = wrapper.DatabaseId; // Associate channel with the client in the database
                        dbContext.Channels.Update(channel);
                    }
                }

                // After associating channels, update the ChannelCount for this client in the database
                var tgClient = await dbContext.TgClients.FindAsync(wrapper.DatabaseId);
                if (tgClient != null)
                {
                    tgClient.ChannelCount = uniqueChannelIds.Count; // Set the channel count
                    dbContext.TgClients.Update(tgClient); // Mark the entity as modified
                }

                // Save changes to the database
                await dbContext.SaveChangesAsync();
            }
        }

        async public Task<Client?> GetClient()
        {
            // Find the first TelegramClientWrapper instance with a matching DatabaseId
            var clientWrapper = Clients.FirstOrDefault();
            return clientWrapper?.Client;
        }

        async public Task<Client?> GetClientByDatabaseId(int databaseId)
        {
            // Find the first TelegramClientWrapper instance with a matching DatabaseId
            var clientWrapper = Clients.FirstOrDefault(c => c.DatabaseId == databaseId);
            return clientWrapper?.Client;
        }

        public async Task<Client?> GetClientByTelegramId(long telegramChannelId)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();

            var pyrogramChannelId = TelegramIdConverter.ToPyrogram(telegramChannelId);

            // Attempt to find the channel in the database
            var dbChannel = await dbContext.Channels
                .Where(c => c.TelegramId == pyrogramChannelId)
                .FirstOrDefaultAsync();

            if (dbChannel == null)
            {
                Console.WriteLine($"Channel with Telegram ID {telegramChannelId} not found in database.");
                return null;
            }

            var clientWrapper = Clients.FirstOrDefault(c => c.DatabaseId == dbChannel.TgclientId);
            if (clientWrapper == null)
            {
                var clientsDbIds = Clients.Select(x => x.DatabaseId).ToList();
                if (clientsDbIds.Count == 0)
                {
                    Console.WriteLine("No clients available to join the channel.");
                    return null;
                }

                // Fetch clients with the lowest channel count from the database
                var lowestChannelCountClient = await dbContext.TgClients
                    .Where(x => clientsDbIds.Contains(x.Id))
                    .OrderBy(c => c.ChannelCount)
                    .FirstOrDefaultAsync();

                if (lowestChannelCountClient == null)
                {
                    Console.WriteLine("No matching clients found in database with provided IDs.");
                    return null;
                }

                clientWrapper = Clients.FirstOrDefault(x => x.DatabaseId == lowestChannelCountClient.Id);

                if (clientWrapper == null)
                {
                    Console.WriteLine($"Client wrapper for the lowest channel count client with ID: {lowestChannelCountClient.Id} not found.");
                    return null;
                }

                var channelNameOrUsername = RemoveTMeUrl(dbChannel.Url);
                if (string.IsNullOrEmpty(channelNameOrUsername))
                {
                    Console.WriteLine("Channel name or username is required but not found.");
                    return null;
                }

                // Now attempt to join the channel
                await TryJoinChannel(clientWrapper.Client, pyrogramChannelId, dbContext, channelNameOrUsername);

                // After joining, increment the channel count
                lowestChannelCountClient.ChannelCount++;
                dbContext.UpdateRange(lowestChannelCountClient, dbChannel);
                await dbContext.SaveChangesAsync();

                // Synchronize the local state
                //clientWrapper.ChannelsCount = lowestChannelCountClient.ChannelCount;

                return clientWrapper.Client;
            }

            return clientWrapper.Client;
        }

        // Auxiliary method assuming existence, implement with appropriate exception handling and logging

        public static string RemoveTMeUrl(string input)
        {
            // Define the pattern to match the URL
            string pattern = @"https?:\/\/t\.me\/";

            // Replace the matched pattern with an empty string
            string result = Regex.Replace(input, pattern, string.Empty);

            return result;
        }

        private async Task TryJoinChannel(Client client, long telegramChannelId, TgDbContext context, string channelUsername)
        {
            try
            {
                var resolveResult = await client.Contacts_ResolveUsername(channelUsername);
                if (resolveResult?.Channel == null)
                {
                    Console.WriteLine($"Channel with username {channelUsername} could not be resolved.");
                    return;
                }

                var inputChannel = new InputChannel(resolveResult.Channel.ID, resolveResult.Channel.access_hash);
                await client.Channels_JoinChannel(inputChannel);
                Console.WriteLine($"Successfully joined channel: {channelUsername}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to join channel {channelUsername}: {ex.Message}");
            }
        }

        public async Task<(long channelId, long accessHash)?> GetChannelAccessHash(long telegramChannelId, Client _client)
        {
            try
            {
                if (_client == null)
                {
                    Console.WriteLine("Failed to get Telegram client.");
                    return null;
                }

                // Convert the channel ID to the expected format
                var pyrogramChannelId = TelegramIdConverter.ToPyrogram(telegramChannelId);

                // Fetch the channel from the database to obtain its username
                var dbChannel = await _dbContextFactory.CreateDbContext().Channels
                                .Where(c => c.TelegramId == pyrogramChannelId)
                                .FirstOrDefaultAsync();

                if (dbChannel == null)
                {
                    Console.WriteLine($"Channel with Telegram ID {telegramChannelId} not found in the database.");
                    return null;
                }

                var channelUsername = RemoveTMeUrl(dbChannel.Url); // Assuming this method extracts the username from the channel URL
                if (string.IsNullOrEmpty(channelUsername))
                {
                    Console.WriteLine("Channel name or username is required but not found.");
                    return null;
                }

                // Resolve the channel using its username to get the access hash
                var resolveResult = await _client.Contacts_ResolveUsername(channelUsername);
                var channel = resolveResult.Channel;

                if (channel != null)
                {
                    Console.WriteLine($"Successfully resolved channel: {channelUsername} with access hash {channel.access_hash}");
                    return (channel.ID, channel.access_hash);
                }
                else
                {
                    Console.WriteLine($"Failed to resolve channel or mismatch in channel ID for username: {channelUsername}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to resolve the channel access hash: {ex.Message}");
            }

            return null;
        }
        public async Task<T> ExecuteWithClientAsync<T>(TelegramClientWrapper clientWrapper, Func<Client, Task<T>> task)
        {
            if (clientWrapper == null)
            {
                throw new ArgumentNullException(nameof(clientWrapper));
            }

            try
            {
                clientWrapper.IsBusy = true;
                return await task(clientWrapper.Client);
            }
            finally
            {
                clientWrapper.IsBusy = false;
            }
        }

        private async Task<List<TL.Message>> GetMessagesByPeriodInternalAsync(Client _client, long channelId, DateTime startDate, DateTime endDate)
        {
            var channelInfo = await GetChannelAccessHash(channelId, _client);
            if (!channelInfo.HasValue)
            {
                throw new InvalidOperationException("Channel not found or access hash unavailable.");
            }

            var (resolvedChannelId, accessHash) = channelInfo.Value;
            var inputPeer = new InputPeerChannel(resolvedChannelId, accessHash);

            var allMessages = new List<TL.Message>();
            int limit = 100;
            DateTime offsetDate = DateTime.UtcNow;
            int lastMessageId = 0;

            while (offsetDate > startDate)
            {
                var messagesBatch = await _client.Messages_GetHistory(inputPeer, offset_id: lastMessageId, limit: limit, offset_date: offsetDate);
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

        public async Task<List<TL.Message>> GetMessagesByPeriodAsync(Client _client, long channelId, DateTime startDate, DateTime endDate)
        {
            var clientWrapper = Clients.FirstOrDefault(c => c.Client == _client);
            if (clientWrapper == null)
            {
                throw new InvalidOperationException("Client wrapper not found.");
            }

            return await ExecuteWithClientAsync(
                clientWrapper,
                async client => await GetMessagesByPeriodInternalAsync(client, channelId, startDate, endDate)
            );
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
