namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate
{
    public class Score : ValueObject
    {
        public Score(double value)
        {
            Value = value;
        }

        public double Value { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
        public static bool operator >(Score left, Score right)
        {
            return left.Value > right.Value;
        }
        public static bool operator <(Score left, Score right)
        {
            return left.Value < right.Value;
        }
        public static bool operator <=(Score left, Score right)
        {
            return left.Value <= right.Value;
        }
        public static bool operator >=(Score left, Score right)
        {
            return left.Value >= right.Value;
        }
        public static Score operator +(Score left, Score right)
        {
            return new Score(left.Value + right.Value);
        }
        public static Score operator -(Score left, Score right)
        {
            return new Score(left.Value - right.Value);
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
