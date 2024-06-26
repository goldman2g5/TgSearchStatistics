using Newtonsoft.Json;

namespace TgSearchStatistics.Models.BaseModels;

public partial class ViewsRecord
{
    public int Id { get; set; }

    public int Views { get; set; }

    public DateTime Date { get; set; }

    public DateTime Updated { get; set; }

    public int Sheet { get; set; }

    public long? LastMessageId { get; set; }
    [JsonIgnore]
    public virtual StatisticsSheet SheetNavigation { get; set; } = null!;
}
