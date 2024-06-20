using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TgSearchStatistics.Models.BaseModels;

public partial class Tag
{
    public int Id { get; set; }

    public string? Text { get; set; }
    [JsonIgnore]
    public virtual ICollection<ChannelHasTag> ChannelHasTags { get; set; } = new List<ChannelHasTag>();
}
