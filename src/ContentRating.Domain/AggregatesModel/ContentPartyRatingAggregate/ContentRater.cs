namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate
{
    public class ContentRater : ValueObject
    {
        public ContentRater(Guid raterId, RaterType raterType)
        {
            RaterId = raterId;
            RaterType = raterType;

        }

        public Guid RaterId { get; }
        public RaterType RaterType { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return RaterId;
            yield return RaterType;
        }      
    }
}
