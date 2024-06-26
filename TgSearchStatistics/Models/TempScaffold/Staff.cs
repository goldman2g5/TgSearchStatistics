namespace TgSearchStatistics.Models.TempScaffold;

public partial class Staff
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual User User { get; set; } = null!;
}
