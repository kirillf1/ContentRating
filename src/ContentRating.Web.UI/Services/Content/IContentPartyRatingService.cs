using ContentRating.Web.Contracts.ContentPartyRating;

namespace ContentRating.Web.UI.Services.Content
{
    public interface IContentPartyRatingService
    {
        Task<ContentPartyRatingResponse?> GetContentRatingAsync(Guid contentRatingId);
        Task<bool> EstimateContentAsync(Guid contentRatingId, EstimateContentRequest request);
    }
}
