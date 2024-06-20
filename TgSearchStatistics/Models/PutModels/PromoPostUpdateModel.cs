namespace TgSearchStatistics.Models
{
    public class PromoPostUpdateModel
    {
        public bool? PromoPost { get; set; }
        public TimeOnly? PromoPostTime { get; set; }
        public int? PromoPostInterval { get; set; }
        public bool? PromoPostSent { get; set; }
        public DateTime? PromoPostLast { get; set; }
    }
}
