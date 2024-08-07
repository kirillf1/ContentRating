namespace ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate
{
    public class Rating(double value) : ValueObject
    {
        public readonly static Rating DefaultMinRating = new(0);
        public readonly static Rating DefaultMaxRating = new(10);
        public double Value { get; } = value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

    }
}
