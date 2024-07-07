using CommandLine;
using Microsoft.EntityFrameworkCore;
using TgSearchStatistics.Interfaces;
using TgSearchStatistics.Models.BaseModels;
using TgSearchStatistics.Utility;

namespace TgSearchStatistics.Services
{
    public class MessageUpdateBackgroundService : BackgroundService
    {
        private readonly ILogger<MessageUpdateBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMessageUpdateQueue _messageUpdateQueue;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1); // Semaphore to ensure single processing

        public MessageUpdateBackgroundService(
            ILogger<MessageUpdateBackgroundService> logger,
            IServiceScopeFactory serviceScopeFactory,
            IMessageUpdateQueue messageUpdateQueue)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _messageUpdateQueue = messageUpdateQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Entering ExecuteAsyncFunction.");

            await foreach (var messages in _messageUpdateQueue.GetReader().ReadAllAsync(stoppingToken))
            {
                _logger.LogInformation("Fetched {Count} messages from the queue.", messages.Count);

                await _semaphore.WaitAsync(stoppingToken); // Ensure single processing
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<TgDbContext>();
                        context.ChangeTracker.Clear();

                        List<TL.Message> batch = new List<TL.Message>();
                        int totalBatches = 0;

                        foreach (var message in messages)
                        {
                            batch.Add(message);

                            if (batch.Count >= 100)
                            {
                                totalBatches++;
                                _logger.LogInformation("Processing batch {BatchNumber} with {BatchCount} messages.", totalBatches, batch.Count);
                                await ProcessBatchAsync(context, batch, stoppingToken);
                                batch.Clear();
                            }
                        }

                        if (batch.Any())
                        {
                            totalBatches++;
                            _logger.LogInformation("Processing batch {BatchNumber} with {BatchCount} messages.", totalBatches, batch.Count);
                            await ProcessBatchAsync(context, batch, stoppingToken);
                        }

                        _logger.LogInformation("Total number of batches processed: {TotalBatches}", totalBatches);
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }

        private async Task ProcessBatchAsync(TgDbContext context, List<TL.Message> batch, CancellationToken stoppingToken)
        {
            _logger.LogInformation("Processing batch of {Count} messages.", batch.Count);

            // Remove duplicates in the batch
            var distinctMessages = batch.GroupBy(m => m.id).Select(g => g.First()).ToList();
            if (distinctMessages.Count != batch.Count)
            {
                _logger.LogWarning("Duplicate messages found and removed. Original count: {OriginalCount}, Distinct count: {DistinctCount}", batch.Count, distinctMessages.Count);
            }

            var messageIds = distinctMessages.Select(m => m.id).ToList();
            _logger.LogInformation("Message IDs to process: {MessageIds}", messageIds);

            var existingMessages = await context.Messages
                .Where(m => messageIds.Contains(m.Id))
                .AsNoTracking()
                .ToDictionaryAsync(m => m.Id, stoppingToken);

            var newMessages = new List<TgSearchStatistics.Models.BaseModels.Message>();
            var updatedMessages = new List<TgSearchStatistics.Models.BaseModels.Message>();

            foreach (var tlMessage in distinctMessages)
            {
                if (existingMessages.TryGetValue(tlMessage.id, out var existingMessage))
                {
                    _logger.LogInformation("Updating existing message ID: {MessageId}", tlMessage.id);
                    existingMessage.Views = tlMessage.views;
                    existingMessage.Text = tlMessage.message;
                    // Update other fields as necessary
                    updatedMessages.Add(existingMessage);
                }
                else
                {
                    _logger.LogInformation("Adding new message ID: {MessageId}", tlMessage.id);
                    var newMessage = new TgSearchStatistics.Models.BaseModels.Message
                    {
                        Id = tlMessage.id,
                        ChannelTelegramId = TelegramUtility.ToPyrogram(tlMessage.peer_id),
                        Views = tlMessage.views,
                        Text = tlMessage.message,
                    };

                    newMessages.Add(newMessage);
                }
            }

            try
            {
                // Detach any existing entities with the same keys before adding/updating
                DetachEntities(context, newMessages);
                DetachEntities(context, updatedMessages);

                if (newMessages.Any())
                {
                    _logger.LogInformation("Adding {Count} new messages.", newMessages.Count);
                    await context.Messages.AddRangeAsync(newMessages, stoppingToken);
                }

                if (updatedMessages.Any())
                {
                    _logger.LogInformation("Updating {Count} messages.", updatedMessages.Count);
                    foreach (var message in updatedMessages)
                    {
                        context.Messages.Attach(message);
                        context.Entry(message).State = EntityState.Modified;
                    }
                }

                _logger.LogInformation("Saving changes to the database.");
                await context.SaveChangesAsync(stoppingToken);
                context.ChangeTracker.Clear();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred: {Message}", ex.Message);
                foreach (var entry in context.ChangeTracker.Entries())
                {
                    _logger.LogError("Entity type: {EntityType}, Entity state: {EntityState}, Message id: {MessageId}", entry.Entity.GetType().Name, entry.State, entry.Entity is TgSearchStatistics.Models.BaseModels.Message msg ? msg.Id : "N/A");
                }
            }
        }

        private void DetachEntities(TgDbContext context, List<TgSearchStatistics.Models.BaseModels.Message> entities)
        {
            foreach (var entity in entities)
            {
                var localEntity = context.Messages.Local.FirstOrDefault(m => m.Id == entity.Id);
                if (localEntity != null)
                {
                    context.Entry(localEntity).State = EntityState.Detached;
                }
            }
        }
    }
}
