using TgSearchStatistics.Models.BaseModels;
using Newtonsoft.Json;

namespace TgSearchStatistics.Models
{
    public class CommentPostModel : Comment
    {
        [System.Text.Json.Serialization.JsonIgnore]
        new public virtual Channel? Channel { get; set; } = null!;
        [System.Text.Json.Serialization.JsonIgnore]
        new public virtual ICollection<Comment>? InverseParent { get; set; } = new List<Comment>();
        [System.Text.Json.Serialization.JsonIgnore]
        new public virtual Comment? Parent { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        new public virtual DateTime CreatedAt
        {
            get { return base.CreatedAt; }
            set { base.CreatedAt = value; }
        }

        [System.Text.Json.Serialization.JsonIgnore]
        new public virtual int UserId {
            get { return base.UserId; }
            set { base.UserId = value; }
        }

        [System.Text.Json.Serialization.JsonIgnore]
        new public virtual User? User { get; set; } = null!;


    }
}
