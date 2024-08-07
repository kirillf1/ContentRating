namespace ContentRatingAPI.Application.YoutubeContent
{
    public class YoutubePlaylist
    {
        public YoutubePlaylist(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public string Name { get; }
        public string Id { get; }
    }
}
