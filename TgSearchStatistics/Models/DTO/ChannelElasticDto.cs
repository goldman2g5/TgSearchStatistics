namespace TgSearchStatistics.Models.DTO
{
    public class ChannelElasticDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public List<string> Tags { get; set; }
    }
}
