﻿namespace ContentRating.Domain.AggregatesModel.ContentRatingAggregate
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
        public bool IsSatisfiedRatersForContentEstimation(Rater initiator, Rater currentRater)
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