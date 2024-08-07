namespace ContentRatingAPI.Application.ContentPartyRating.EstimateContent
{
    public class EstimateContentRequest
    {
        public required double NewScore { get; set; }
        public required Guid RaterForChangeScoreId { get; set; }
    }
}
