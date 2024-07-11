using CommandLine;
using Microsoft.EntityFrameworkCore;
using TgSearchStatistics.Interfaces;
using TgSearchStatistics.Models.BaseModels;
using TgSearchStatistics.Utility;
using System.Diagnostics;

namespace TgSearchStatistics.Services
{

    public class MessageUpdateBackgroundService : BackgroundService
    {
        private readonly ILogger<MessageUpdateBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMessageUpdateQueue _messageUpdateQueue;

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

            long totalBatchProcessingTime = 0;

            try
            {
                await foreach (var messages in _messageUpdateQueue.GetReader().ReadAllAsync(stoppingToken))
                {
                    _logger.LogInformation("Fetched {Count} messages from the queue.", messages.Count);

                    var distinctMessages = messages
                        .GroupBy(m => m.id)
                        .Select(g => g.First())
                        .ToList();

                    var batches = distinctMessages
                        .Select((message, index) => new { message, index })
                        .GroupBy(x => x.index / 100)
                        .Select(g => g.Select(x => x.message).ToList())
                        .ToList();

                    _logger.LogInformation("Processing {BatchCount} batches.", batches.Count);

                    var batchProcessingStopwatch = Stopwatch.StartNew();

                    var tasks = batches.Select(batch => ProcessBatchAsync(batch, stoppingToken)).ToList();
                    await Task.WhenAll(tasks);

                    batchProcessingStopwatch.Stop();
                    var batchProccesingTimeElapsed = batchProcessingStopwatch.ElapsedMilliseconds;
                    totalBatchProcessingTime += batchProccesingTimeElapsed;

                    _logger.LogInformation("Batch Execution Time: {ElapsedMilliseconds} ms", batchProcessingStopwatch.ElapsedMilliseconds);
                    _logger.LogInformation("Total time for processing all batches: {ElapsedMilliseconds} ms", totalBatchProcessingTime);
                }
            }
            finally
            {
            }
        }

        private async Task ProcessBatchAsync(List<TL.Message> batch, CancellationToken stoppingToken)
        {
            var batchStopwatch = Stopwatch.StartNew();

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TgDbContext>();
                context.ChangeTracker.Clear();

                _logger.LogInformation("Processing batch of {Count} messages.", batch.Count);

                var messageIds = batch.Select(m => m.id).ToList();

                var existingMessages = await context.Messages
                    .Where(m => messageIds.Contains(m.Id))
                    .AsNoTracking()
                    .ToDictionaryAsync(m => m.Id, stoppingToken);

                var newMessages = new List<TgSearchStatistics.Models.BaseModels.Message>();
                var updatedMessages = new List<TgSearchStatistics.Models.BaseModels.Message>();

                foreach (var tlMessage in batch)
                {
                    if (existingMessages.TryGetValue(tlMessage.id, out var existingMessage))
                    {
                        existingMessage.Views = tlMessage.views;
                        existingMessage.Text = tlMessage.message;
                        updatedMessages.Add(existingMessage);
                    }
                    else
                    {
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
                    if (newMessages.Any())
                    {
                        await context.Messages.AddRangeAsync(newMessages, stoppingToken);
                    }

                    if (updatedMessages.Any())
                    {
                        foreach (var message in updatedMessages)
                        {
                            context.Messages.Attach(message);
                            context.Entry(message).State = EntityState.Modified;
                        }
                    }

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

            batchStopwatch.Stop();
            _logger.LogInformation("Batch processed in {ElapsedMilliseconds} ms", batchStopwatch.ElapsedMilliseconds);
        }
    }
}
