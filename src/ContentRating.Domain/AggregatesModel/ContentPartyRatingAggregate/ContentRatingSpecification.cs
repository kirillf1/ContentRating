namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate
{
    public class ContentRatingSpecification
    {
        public ContentRatingSpecification(Score minScore, Score maxScore)
        {
            if (minScore > maxScore)
                throw new ArgumentException("Max score should be more than min");
            MinScore = minScore;
            MaxScore = maxScore;
        }

        public Score MinScore { get; }
        public Score MaxScore { get; }
        public bool IsSatisfiedScore(Score score)
        {
            return score >= MinScore && score <= MaxScore;
        }
        public bool IsSatisfiedRatersForContentEstimation(ContentRater initiator, ContentRater currentRater)
        {
            if (initiator == currentRater)
                return true;

            if (initiator.RaterType == RaterType.Mock)
                return false;

            if (initiator.RaterType != RaterType.Admin && currentRater.RaterType != RaterType.Mock)
                return false;

            return true;
        }
    }
}
