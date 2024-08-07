namespace ContentRatingAPI.Infrastructure.YoutubeClient.Models
{
    public class Item
    {
        public string? Kind { get; set; }
        public string? Etag { get; set; }
        public string? Id { get; set; }
        public Snippet? Snippet { get; set; }
    }
}
