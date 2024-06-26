namespace TgSearchStatistics.Models.TempScaffold;

public partial class ChannelHasSubscription
{
    public int Id { get; set; }

    public int? TypeId { get; set; }

    public DateTime? Expires { get; set; }

    public int? ChannelId { get; set; }

    public virtual Channel? Channel { get; set; }

    public virtual SubType? Type { get; set; }
}
