using Newtonsoft.Json;
using TgSearchStatistics.Models.BaseModels;

namespace TgSearchStatistics.Models.GetModels
{
    public class ReportGetModel : Report
    {
        public string ChannelName { get; set; }

        public string ChannelUrl { get; set; }

        public string ChannelWebUrl { get; set; }

        public string ReporteeName { get; set; }

        public long? UserTelegramChatId { get; set; }
    }
}
