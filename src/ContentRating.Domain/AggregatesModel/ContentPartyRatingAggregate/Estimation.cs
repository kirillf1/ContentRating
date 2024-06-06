using ContentRating.Domain.Shared.RoomStates;

namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate
{
    public class Estimation : ValueObject
    {
        public Estimation(ContentRater initiator, ContentRater currentRater, Score newScore)
        {
            Initiator = initiator;
            CurrentRater = currentRater;
            NewScore = newScore;
        }

        public ContentRater Initiator { get; }
        public ContentRater CurrentRater { get; }
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
