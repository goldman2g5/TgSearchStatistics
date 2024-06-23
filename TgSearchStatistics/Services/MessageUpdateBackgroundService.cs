using CommandLine;
using Microsoft.Diagnostics.Tracing.Parsers.AspNet;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;
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
                var messageBatches = messages
                    .Select((message, index) => new { message, index })
                    .GroupBy(x => x.index / 100)
                    .Select(g => g.Select(x => x.message).ToList());

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<TgDbContext>();
                    context.ChangeTracker.Clear();

                    foreach (var batch in messageBatches)
                    {
                        _logger.LogInformation("Starting processing batch of messages.");

                        var messageIds = batch.Select(m => m.id).ToList();
                        _logger.LogInformation("Message IDs to process: {MessageIds}", messageIds);

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
                                _logger.LogInformation("Updating existing message ID: {MessageId}", tlMessage.id);
                                var updatedMessage = new TgSearchStatistics.Models.BaseModels.Message
                                {
                                    Id = existingMessage.Id,
                                    ChannelTelegramId = existingMessage.ChannelTelegramId,
                                    Views = tlMessage.views,
                                    Text = tlMessage.message,
                                    // Continue updating other fields as necessary
                                };
                                updatedMessages.Add(updatedMessage);
                            }
                            else if (!context.Messages.Local.Any(m => m.Id == tlMessage.id))
                            {
                                _logger.LogInformation("Adding new message ID: {MessageId}", tlMessage.id);
                                newMessages.Add(new TgSearchStatistics.Models.BaseModels.Message
                                {
                                    Id = tlMessage.id,
                                    ChannelTelegramId = TelegramIdConverter.ToPyrogram(tlMessage.peer_id),
                                    Views = tlMessage.views,
                                    Text = tlMessage.message,
                                });
                            }
                            else
                            {
                                _logger.LogWarning("Message ID: {MessageId} is already being tracked and will not be added again.", tlMessage.id);
                            }
                        }

                        try
                        {
                            if (newMessages.Any())
                            {
                                _logger.LogInformation("Adding {Count} new messages.", newMessages.Count);
                                await context.Messages.AddRangeAsync(newMessages, stoppingToken);
                            }

                            if (updatedMessages.Any())
                            {
                                _logger.LogInformation("Updating {Count} messages.", updatedMessages.Count);
                                context.Messages.UpdateRange(updatedMessages); // Use UpdateRange to handle tracking
                            }

                            _logger.LogInformation("Saving changes to the database.");
                            await context.SaveChangesAsync(stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("An error occurred: {Message}", ex.Message);
                            foreach (var entry in context.ChangeTracker.Entries())
                            {
                                _logger.LogError("Entity type: {EntityType}, Entity state: {EntityState}, Message id: {MessageId}", entry.Entity.GetType().Name, entry.State, entry.Entity.Cast<Message>().Id);
                            }
                        }
                    }
                }
            }
        }
    }
}
