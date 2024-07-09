using ContentRating.Domain.Shared.Content;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.GetPartyEstimationRoom
{
    public class ContentRatingResponse
    {
        public ContentRatingResponse(Guid ratingId, Guid contentId, string name, string address, ContentType contentType, IEnumerable<RatingByRaterResponse> ratings, double averageRating)
        {
            RatingId = ratingId;
            ContentId = contentId;
            Name = name;
            Address = address;
            ContentType = contentType;
            Ratings = ratings;
            AverageRating = averageRating;
        }

        public Guid RatingId { get; }
        public Guid ContentId { get; }
        public string Name { get; }
        public string Address { get; }
        public ContentType ContentType { get; }
        public IEnumerable<RatingByRaterResponse> Ratings { get; }
        public double AverageRating { get; }
    }
   
}
