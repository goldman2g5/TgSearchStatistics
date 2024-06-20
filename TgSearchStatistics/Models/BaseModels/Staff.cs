using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TgSearchStatistics.Models.BaseModels;

public partial class Staff
{
    public int Id { get; set; }

    public int UserId { get; set; }
    [JsonIgnore]
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
