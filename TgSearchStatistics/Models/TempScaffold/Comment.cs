using System;
using System.Collections.Generic;

namespace TgSearchStatistics.Models.TempScaffold;

public partial class Comment
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public int UserId { get; set; }

    public int ChannelId { get; set; }

    public int? ParentId { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? Rating { get; set; }

    public virtual Channel Channel { get; set; } = null!;

    public virtual ICollection<Comment> InverseParent { get; set; } = new List<Comment>();

    public virtual Comment? Parent { get; set; }

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual User User { get; set; } = null!;
}
