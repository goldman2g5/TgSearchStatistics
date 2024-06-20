using System;
using System.Collections.Generic;

namespace TgSearchStatistics.Models.BaseModels;

public partial class ChannelAccess
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? ChannelId { get; set; }

    public virtual Channel? Channel { get; set; }

    public virtual User? User { get; set; }
}
