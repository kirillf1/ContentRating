namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate
{
    public class Estimation : ValueObject
    {
        public Estimation(ContentRater estimationInitiator, ContentRater raterForChangeScore, Score newScore)
        {
            ContentEstimationInitiator = estimationInitiator;
            RaterForChangeScore = raterForChangeScore;
            NewScore = newScore;
        }

        public ContentRater ContentEstimationInitiator { get; }
        public ContentRater RaterForChangeScore { get; }
        public Score NewScore { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ContentEstimationInitiator;
            yield return RaterForChangeScore;
            yield return NewScore;
        }
    }
}
