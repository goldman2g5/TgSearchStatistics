using System;
using System.Collections.Generic;

namespace TgSearchStatistics.Models.BaseModels;

public partial class ChannelHasTag
{
    public int Id { get; set; }

    public int? Tag { get; set; }

    public int? Channel { get; set; }

    public virtual Channel? ChannelNavigation { get; set; }

    public virtual Tag? TagNavigation { get; set; }
}
