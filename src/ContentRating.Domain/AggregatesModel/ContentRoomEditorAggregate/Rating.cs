using ContentRating.Domain.Shared;

namespace ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate
{
    public class Rating(double value) : ValueObject
    {
        public static Rating DefaultMinRating = new(0);
        public static Rating DefaultMaxRating = new(10);
        public double Value { get; } = value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

    }
}
