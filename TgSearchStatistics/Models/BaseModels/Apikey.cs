namespace TgSearchStatistics.Models.BaseModels;

public partial class Apikey
{
    public int Id { get; set; }

    public string ClientName { get; set; } = null!;

    public string Key { get; set; } = null!;
}
