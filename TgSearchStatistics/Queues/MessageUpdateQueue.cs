using System.Threading.Channels;
using TgSearchStatistics.Interfaces;

namespace TgSearchStatistics.Queues
{
    public class MessageUpdateQueue : IMessageUpdateQueue
    {
        private readonly Channel<List<TL.Message>> _messageChannel;

        public MessageUpdateQueue()
        {
            _messageChannel = Channel.CreateUnbounded<List<TL.Message>>();
        }

        public async Task QueueMessagesForUpdateAsync(List<TL.Message> messages)
        {
            await _messageChannel.Writer.WriteAsync(messages);
        }

        public ChannelReader<List<TL.Message>> GetReader() => _messageChannel.Reader;
    }
}
