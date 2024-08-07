namespace ContentRatingAPI.Application.ContentPartyRating.GetContentRating
{
    public class RatingResponse
    {
        public RatingResponse(Guid raterId, double rating)
        {
            RaterId = raterId;
            Rating = rating;
        }

        public Guid RaterId { get; }
        public double Rating { get; }
    }
}
