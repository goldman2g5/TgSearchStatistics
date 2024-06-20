using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TgSearchStatistics.Models.BaseModels;

public partial class Comment
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public int UserId { get; set; }

    public int ChannelId { get; set; }

    public int? ParentId { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? Rating { get; set; }
    [JsonIgnore]
    public virtual Channel Channel { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Comment> InverseParent { get; set; } = new List<Comment>();
    [JsonIgnore]
    public virtual Comment? Parent { get; set; }
    [JsonIgnore]
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
