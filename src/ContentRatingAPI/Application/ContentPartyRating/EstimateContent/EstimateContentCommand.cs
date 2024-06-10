namespace ContentRatingAPI.Application.ContentPartyRating.EstimateContent
{
    public record EstimateContentCommand(Guid ContentId, Guid EstimationInitiatorId, Guid RaterForChangeScoreId, double NewScore) : IRequest;

}
