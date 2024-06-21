using System.Threading.Channels;

namespace TgSearchStatistics.Interfaces
{
    public interface IMessageUpdateQueue
    {
        Task QueueMessagesForUpdateAsync(List<TL.Message> messages);

        ChannelReader<List<TL.Message>> GetReader();

    }
}
