namespace ContentRatingAPI.Application.YoutubeContent
{
    public interface IYoutubeClient
    {
        public Task<Result<IEnumerable<YoutubeVideo>>> GetVideosFromPlayList(string playListId, string accessToken);
        public Task<Result<IEnumerable<YoutubePlaylist>>> GetAvailablePlayLists(string accessToken);
    }
}
