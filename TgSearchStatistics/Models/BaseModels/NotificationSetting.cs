namespace TgSearchStatistics.Models.BaseModels;

public partial class NotificationSetting
{
    public int Id { get; set; }

    public bool Bump { get; set; }

    public bool General { get; set; }

    public bool Important { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
