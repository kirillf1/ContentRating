
namespace ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate
{
    public class RatingRange : ValueObject
    {
        public RatingRange(Rating maxRating, Rating minRating)
        {
            if (minRating.Value > maxRating.Value)
                throw new ArgumentException("Min rating can't be more than max rating");
            MaxRating = maxRating;
            MinRating = minRating;
        }
        public RatingRange()
        {
            MaxRating = Rating.DefaultMaxRating;
            MinRating = Rating.DefaultMinRating;
        }

        public Rating MaxRating { get; }
        public Rating MinRating { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return MaxRating;
            yield return MinRating;
        }
    }
}
