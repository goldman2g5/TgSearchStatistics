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
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMessageUpdateQueue _messageUpdateQueue;

        public MessageUpdateBackgroundService(IServiceScopeFactory serviceScopeFactory, IMessageUpdateQueue messageUpdateQueue)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _messageUpdateQueue = messageUpdateQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var messages in _messageUpdateQueue.GetReader().ReadAllAsync(stoppingToken))
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<TgDbContext>();

                var messageIds = messages.Select(m => m.id).ToList();
                var existingMessages = await context.Messages
                    .Where(m => messageIds.Contains(m.Id))
                    .ToListAsync(stoppingToken);

                foreach (var tlMessage in messages)
                {
                    var existingMessage = existingMessages.FirstOrDefault(m => m.Id == tlMessage.id);
                    if (existingMessage == null)
                    {
                        context.Messages.Add(new TgSearchStatistics.Models.BaseModels.Message
                        {
                            Id = tlMessage.id,
                            ChannelTelegramId = TelegramIdConverter.ToPyrogram(tlMessage.peer_id),
                            Views = tlMessage.views,
                            Text = tlMessage.message,
                            // Map other fields as necessary
                        });
                    }
                    else
                    {
                        // Update fields as necessary
                        existingMessage.Views = tlMessage.views;
                        existingMessage.Text = tlMessage.message;
                        // Continue updating other fields as necessary
                    }
                }

                await context.SaveChangesAsync(stoppingToken);
            }
        }
    }

}
