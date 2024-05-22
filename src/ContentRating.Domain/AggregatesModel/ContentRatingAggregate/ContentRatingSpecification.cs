namespace ContentRating.Domain.AggregatesModel.ContentRatingAggregate
{
    public class ContentRatingSpecification
    {
        public ContentRatingSpecification(Score minScore, Score maxScore)
        {
            MinScore = minScore;
            MaxScore = maxScore;
        }

        public Score MinScore { get; }
        public Score MaxScore { get; }
        public bool IsSatisfiedScore(Score score)
        {
            return MinScore >= score && score <= MaxScore;
        }
        public bool IsSatisfiedRaters(Rater initiator, Rater currentRater)
        {
            if(initiator == currentRater)
                return true;

            if (initiator.RaterType == RaterType.Mock)
                return false;

            if(initiator.RaterType != RaterType.Owner && currentRater.RaterType != RaterType.Mock)
                return false;

            return true;
        }
    }
}
