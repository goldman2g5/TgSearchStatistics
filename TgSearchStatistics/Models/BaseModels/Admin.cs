using System;
using System.Collections.Generic;

namespace TgSearchStatistics.Models.BaseModels;

public partial class Admin
{
    public int Id { get; set; }

    public string Key { get; set; } = null!;

    public long? TelegramId { get; set; }
}
