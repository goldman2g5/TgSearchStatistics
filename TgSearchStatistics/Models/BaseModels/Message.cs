namespace TgSearchStatistics.Models.BaseModels;

public partial class Message
{
    public int Id { get; set; }

    public long ChannelTelegramId { get; set; }

    public int? Views { get; set; }

    public string? Text { get; set; }

    public string? Media { get; set; }

    public virtual Channel ChannelTelegram { get; set; } = null!;
}
