namespace TgSearchStatistics.Models.PostModels
{
    public class CreateNotificationPostModel
    {
        public int channelid { get; set; }
        public string content { get; set; }
        public int userid { get; set; }
        public int typeid { get; set; }
    }
}
