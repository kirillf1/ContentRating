using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.Exceptions;

namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate
{
    public class ContentRater : Entity
    {
        public ContentRater(Guid id, RaterType raterType, Score score)
        {
            Id = id;
            RaterType = raterType;
            CurrentScore = score;
        }
        public RaterType RaterType { get; private set; }
        public Score CurrentScore { get; private set; }
        internal void Rate(Score score, ContentRatingSpecification ratingSpecification)
        {
            if (!ratingSpecification.IsSatisfiedScore(score))
                throw new ForbiddenRatingOperationException($"Score must be more or equal {ratingSpecification.MinScore.Value} " +
                    $"and less or equal {ratingSpecification.MaxScore.Value}");
            CurrentScore = score;
        }
    }
}
