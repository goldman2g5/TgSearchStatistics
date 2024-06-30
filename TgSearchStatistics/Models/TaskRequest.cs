using WTelegram;

namespace TgSearchStatistics.Models
{
    public class TaskRequest
    {
        public long ChannelId { get; set; }
        public string ChannelUsername { get; set; }
        public Func<Client, Task<List<TL.Message>>> TaskToExecute { get; set; }
        public TaskCompletionSource<List<TL.Message>> TaskCompletionSource { get; set; }
    }
}
