namespace ContentRatingAPI.Application.ContentPartyRating.GetContentRating
{
    public record class GetContentRatingQuery(Guid RatingId): IRequest<Result<ContentPartyRatingResponse>>;
   
}
