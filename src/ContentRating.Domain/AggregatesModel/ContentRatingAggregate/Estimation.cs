using ContentRating.Domain.Shared;
using ContentRating.Domain.Shared.RoomStates;

namespace ContentRating.Domain.AggregatesModel.ContentRatingAggregate
{
    public class Estimation : ValueObject
    {
        public Estimation(Rater initiator, Rater currentRater, Score newScore)
        {
            Initiator = initiator;
            CurrentRater = currentRater;
            NewScore = newScore;
        }

        public Rater Initiator { get; }
        public Rater CurrentRater { get; }
        public Score NewScore { get; }
        public RoomState RoomState { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Initiator;
            yield return CurrentRater;
            yield return NewScore;
        }
    }
}
