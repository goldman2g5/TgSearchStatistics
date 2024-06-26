namespace TgSearchStatistics.Models.BaseModels;

public partial class TgClient
{
    public int Id { get; set; }

    public string? ApiId { get; set; }

    public string? ApiHash { get; set; }

    public string? PhoneNumber { get; set; }

    public int? ChannelCount { get; set; }

    public long? TelegramId { get; set; }

    public virtual ICollection<Channel> Channels { get; set; } = new List<Channel>();
}
