namespace ContentRatingAPI.Application.ContentPartyRating.GetContentRating
{
    public class ContentPartyRatingResponse
    {
        public ContentPartyRatingResponse(Guid id, double averageRating, IEnumerable<RatingResponse> ratings)
        {
            Id = id;
            AverageRating = averageRating;
            Ratings = ratings;
        }

        public Guid Id { get; }
        public double AverageRating { get; }
        public IEnumerable<RatingResponse> Ratings { get; }
    }
}
