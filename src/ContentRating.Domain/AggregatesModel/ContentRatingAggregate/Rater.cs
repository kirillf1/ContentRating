using ContentRating.Domain.AggregatesModel.ContentRatingAggregate.Exceptions;
using ContentRating.Domain.Shared;

namespace ContentRating.Domain.AggregatesModel.ContentRatingAggregate
{
    public class Rater : Entity
    {
        public Rater(Guid id, RaterType raterType, Score score)
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
