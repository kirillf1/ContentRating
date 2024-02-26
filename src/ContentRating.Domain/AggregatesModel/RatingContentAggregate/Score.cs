using ContentRating.Domain.Shared;

namespace ContentRating.Domain.AggregatesModel.RatingContentAggregate
{
    public class Score : ValueObject
    {
        public static readonly Score DefaultScore = new(0);
        public Score(double value) 
        {
            if (value < 0 || value > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(value), message: "Score must be equals or more than 0 and equals or less than 10");
            }
            Value = value;
        }

        public double Value { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
