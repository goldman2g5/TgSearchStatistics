namespace TgSearchStatistics.Models.TempScaffold;

public partial class SubType
{
    public int Id { get; set; }

    public int? Price { get; set; }

    public decimal? Multiplier { get; set; }

    public string? Name { get; set; }

    public int TagLimit { get; set; }

    public virtual ICollection<ChannelHasSubscription> ChannelHasSubscriptions { get; set; } = new List<ChannelHasSubscription>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<TelegramPayment> TelegramPayments { get; set; } = new List<TelegramPayment>();
}
