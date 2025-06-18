using ContentRating.Web.Contracts.YoutubeContent;

namespace ContentRatingAPI.Application.YoutubeContent.GetYoutubeVideoTitle
{
    public record class GetYoutubeVideoTitleQuery(string Url) : IRequest<Result<YoutubeVideoTitle>>;
}
