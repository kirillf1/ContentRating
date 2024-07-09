namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.GetPartyEstimationRoom
{
    public class RatingByRaterResponse
    {
        public RatingByRaterResponse(Guid raterId, double rating)
        {
            RaterId = raterId;
            Rating = rating;
        }

        public Guid RaterId { get; }
        public double Rating { get; }
    }
   
}
