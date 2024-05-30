namespace ContentRatingAPI.Application.ContentRating.EstimateContent
{
    public record EstimateContentCommand(Guid ContentId, Guid RaterInitiator, Guid TargetRater, double NewScore): IRequest;
      
}
