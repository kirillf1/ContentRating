namespace ContentRatingAPI.Application.YoutubeContent
{
    public class YoutubeVideo
    {
        public YoutubeVideo(string url, string urlEmbed, string name)
        {
            Url = url;
            UrlEmbed = urlEmbed;
            Name = name;
        }

        public string Url { get; }
        public string UrlEmbed { get; }
        public string Name { get; }
    }
}
