using System;
using System.Collections.Generic;

namespace TgSearchStatistics.Models.TempScaffold;

public partial class Tag
{
    public int Id { get; set; }

    public string? Text { get; set; }

    public virtual ICollection<ChannelHasTag> ChannelHasTags { get; set; } = new List<ChannelHasTag>();
}
