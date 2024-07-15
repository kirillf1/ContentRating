namespace ContentRatingAPI.Infrastructure.YoutubeClient.Models
{
    public class PlaylistModel
    {
        public string? Kind { get; set; }
        public string? Etag { get; set; }
        public string? NextPageToken { get; set; }
        public List<Item>? Items { get; set; }
        public PageInfo? PageInfo { get; set; }
    }
}
