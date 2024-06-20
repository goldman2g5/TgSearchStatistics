namespace TgSearchStatistics.Models.NotificationModels
{
    public class ReportNotificationSupport
    {
        public int ReportId { get; set; }

        public string ReporteeName { get; set; }

        public int ReporteeId { get; set; }

        public string ChannelName { get; set; }

        public int ChannelId { get; set; }

        public List<long?> Targets { get; set; }
    }
}
