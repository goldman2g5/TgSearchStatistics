using TgSearchStatistics.Models.GetModels;

namespace TgSearchStatistics.Models.BaseModels
{
    public class ReportGroup
    {
        public int ChannelId { get; set; }

        public string ChannelName { get; set; }

        public string ChannelUrl { get; set; }

        public string ChannelWebUrl { get; set; }

        public DateTime? LastReport { get; set; }

        public int ReportCount { get; set; }

        public List<ReportGetModel> Reports { get; set; }
    }
}
