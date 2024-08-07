namespace ContentRatingAPI.Application.YoutubeContent.GetYoutubePlayLists
{
    public class GetYoutubePlayListsQuery: IRequest<Result<IEnumerable<YoutubePlaylist>>>
    {
        public GetYoutubePlayListsQuery(Guid userId)
        {
            UserId = userId;
        }

        public Guid UserId { get; }
    }
}
