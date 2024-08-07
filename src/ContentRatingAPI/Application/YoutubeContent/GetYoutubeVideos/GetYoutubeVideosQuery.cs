namespace ContentRatingAPI.Application.YoutubeContent.GetYoutubeVideos
{
    public record class GetYoutubeVideosQuery(Guid UserId, string PlaylistId) : IRequest<Result<IEnumerable<YoutubeVideo>>>;
}
