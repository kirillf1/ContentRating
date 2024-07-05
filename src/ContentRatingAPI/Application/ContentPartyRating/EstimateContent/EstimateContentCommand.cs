namespace ContentRatingAPI.Application.ContentPartyRating.EstimateContent
{
    public record EstimateContentCommand(Guid ContentRatingId, Guid EstimationInitiatorId, 
        Guid RaterForChangeScoreId, double NewScore) : IRequest<Result>;

}
