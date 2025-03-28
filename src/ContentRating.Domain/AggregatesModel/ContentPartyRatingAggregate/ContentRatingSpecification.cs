// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate
{
    public class ContentRatingSpecification
    {
        public ContentRatingSpecification(Score minScore, Score maxScore)
        {
            if (minScore > maxScore)
            {
                throw new ArgumentException("Max score should be more than min");
            }

            MinScore = minScore;
            MaxScore = maxScore;
        }

        public Score MinScore { get; }
        public Score MaxScore { get; }

        public bool IsSatisfiedScore(Score score)
        {
            return score >= MinScore && score <= MaxScore;
        }

        public bool HasAccessToEstimateContent(ContentRater estimationInitiator, ContentRater raterForChangeScore)
        {
            if (estimationInitiator == raterForChangeScore)
            {
                return true;
            }

            if (estimationInitiator.RaterType != RaterType.Mock && raterForChangeScore.RaterType == RaterType.Mock)
            {
                return true;
            }

            return false;
        }
    }
}
